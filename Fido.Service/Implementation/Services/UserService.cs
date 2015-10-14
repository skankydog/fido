using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Core;
using Fido.Dtos;
using Fido.DataAccess;
using Fido.Service.Exceptions;

namespace Fido.Service.Implementation
{
    internal class UserService : CRUDService<User, Entities.User, DataAccess.IUserRepository>, IUserService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<Guid, IList<Activity>> PermissionCache = new Dictionary<Guid, IList<Activity>>(); // TO DO: not necessary

        #region Pages
        public IList<User> GetPageInDefaultOrder(int Skip, int Take, char SortDirection, string Filter)
        {
            return GetPage(Skip, Take, SortDirection, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Id),
                OrderByDescending: q => q.OrderByDescending(s => s.Id));
        }

        public IList<User> GetPageInFirstnameOrder(int Skip, int Take, char SortDirection, string Filter)
        {
            return GetPage(Skip, Take, SortDirection, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Fullname.Firstname),
                OrderByDescending: q => q.OrderByDescending(s => s.Fullname.Firstname));
        }

        public IList<User> GetPageInSurnameOrder(int Skip, int Take, char SortDirection, string Filter)
        {
            return GetPage(Skip, Take, SortDirection, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Fullname.Surname),
                OrderByDescending: q => q.OrderByDescending(s => s.Fullname.Surname));
        }

        public IList<User> GetPageInEmailAddressOrder(int Skip, int Take, char SortDirection, string Filter)
        {
            return GetPage(Skip, Take, SortDirection, Filter,
                OrderByAscending: q => q.OrderBy(s => s.EmailAddress),
                OrderByDescending: q => q.OrderByDescending(s => s.EmailAddress));
        }

        public IList<User> GetPageInLocalCredentialOrder(int Skip, int Take, char SortDirection, string Filter)
        {
            return GetPage(Skip, Take, SortDirection, Filter,
                OrderByAscending: q => q.OrderBy(s => s.LocalCredentialState),
                OrderByDescending: q => q.OrderByDescending(s => s.LocalCredentialState));
        }

        public IList<User> GetPageInExternalCredentialOrder(int Skip, int Take, char SortDirection, string Filter)
        {
            return GetPage(Skip, Take, SortDirection, Filter,
                OrderByAscending: q => q.OrderBy(s => s.ExternalCredentialState),
                OrderByDescending: q => q.OrderByDescending(s => s.ExternalCredentialState));
        }

        private IList<User> GetPage(int Skip, int Take, char SortDirection, string Filter,
            Func<IQueryable<Entities.User>, IOrderedQueryable<Entities.User>> OrderByAscending,
            Func<IQueryable<Entities.User>, IOrderedQueryable<Entities.User>> OrderByDescending)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var OrderBy = SortDirection == 'a' ? OrderByAscending : OrderByDescending;
                    var Query = UserRepository.GetAsIQueryable(e => e.Id != null, OrderBy);

                    if (Filter.IsNotNullOrEmpty())
                    {
                        Query = Query.Where(e => e.Fullname.Firstname.ToLower().Contains(Filter.ToLower())
                                              || e.Fullname.Surname.ToLower().Contains(Filter.ToLower())
                                              || e.EmailAddress.ToLower().Contains(Filter.ToLower()));
                    }

                    Query = Query.Skip(Skip/* * Take*/).Take(Take);
                    
                    var EntityList = Query.ToList(); // Hit the database

                    IList<User> DtoList = AutoMapper.Mapper.Map<IList<Entities.User>, IList<User>>(EntityList);
                    return DtoList;
                }
            }
        }
        #endregion

        #region Change Email Address
        public Guid InitiateChangeEmailAddress(Guid UserId, string NewEmailAddress)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                if (AuthenticationService.EmailAddressIsFree(NewEmailAddress) == false)
                    throw new EmailAddressDuplicationException(NewEmailAddress);

                if (AuthenticationService.EmailAddressPassesValidation(NewEmailAddress) == false)
                    throw new EmailAddressValidationException(NewEmailAddress);

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new ServiceException("Failed to retrieve user details");

                    Guid ConfirmationId = ConfirmationService.QueueConfirmation(UnitOfWork, "Change Email Address", UserId, NewEmailAddress);
                    UserEntity.CurrentLocalCredentialState.InitiateChangeEmailAddress();

                    UnitOfWork.Commit();
                    return ConfirmationId;
                }
            }
        }

        public User CompleteChangeEmailAddress(Guid ConfirmationId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var Confirmation = ConfirmationService.ReceiveConfirmation(UnitOfWork, ConfirmationId, "Change Email Address");

                    if (Confirmation == null)
                        throw new ServiceException(string.Format("Unable to change email address - {0} not found or expired", ConfirmationId));

                    var UserEntity = UserRepository.Get(Confirmation.UserId);

                    UserEntity.CurrentLocalCredentialState.CompleteChangeEmailAddress(Confirmation.EmailAddress);

                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return AutoMapper.Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }
        #endregion

        #region Change Password/Expire Credentials
        public User ChangeLocalPassword(Guid UserId, string OldPassword, string NewPassword)
        {
            using (new FunctionLogger(Log))
            {
                if (PasswordAdvisor.WeakOrInvalid(NewPassword))
                    throw new PasswordValidationException();

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(e => e.Id == UserId && e.Password == OldPassword);

                    if (UserEntity == null)
                        throw new ServiceException(string.Format("User id, {0}, does not exist or incorrect password entered", UserId));

                    UserEntity.CurrentLocalCredentialState.ChangePassword(NewPassword);
                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return AutoMapper.Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }

        public User ExpireLocalCredentials(Guid UserId)
        {
            using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                var UserEntity = UserRepository.Get(UserId);

                if (UserEntity == null)
                    throw new ServiceException(string.Format("User id, {0}, does not exist", UserId));

                UserEntity.CurrentLocalCredentialState.Expire();
                UserRepository.Update(UserEntity);
                UnitOfWork.Commit();

                return AutoMapper.Mapper.Map<Entities.User, Dtos.User>(UserEntity);
            }
        }
        #endregion

        #region Email Address
        public User GetByEmailAddress(string EmailAddress)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = Repository.Get(e => e.EmailAddress == EmailAddress, "Roles");

                    User UserDTO = AutoMapper.Mapper.Map<Entities.User, User>(UserEntity);

                    return UserDTO;
                }
            }
        }
        #endregion

        #region Profile
        public Profile GetProfile(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId, "ProfileImage");

                    return AutoMapper.Mapper.Map<Entities.User, Profile>(UserEntity);
                }
            }
        }

        public void SetProfile(Profile Profile)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(Profile.Id);

                    UserEntity = AutoMapper.Mapper.Map<Profile, Entities.User>(Profile, UserEntity);
                    UserRepository.Update(UserEntity);

                    if (Profile.Image != null) // Post contains an image
                    {
                        var ProfileImageRepository = DataAccessFactory.CreateRepository<IProfileImageRepository>(UnitOfWork);
                        var ProfileImageEntity = ProfileImageRepository.Get(Profile.Id);

                        if (ProfileImageEntity == null) // No image in the database
                        {
                            ProfileImageEntity = new Entities.ProfileImage
                                {
                                    Id = Profile.Id,
                                    Image = Profile.Image
                                };

                            ProfileImageRepository.Insert(ProfileImageEntity);
                        }
                        else // Overwrite the image in the database
                        {
                            ProfileImageEntity = AutoMapper.Mapper.Map<Profile, Entities.ProfileImage>(Profile, ProfileImageEntity);
                            ProfileImageRepository.Update(ProfileImageEntity);
                        }
                    }

                    UnitOfWork.Commit();
                }
            }
        }
        #endregion

        #region Settings
        public Settings GetSettings(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId, "ExternalCredentials");

                    var ConfigurationRepository = DataAccessFactory.CreateRepository<IConfigurationRepository>(UnitOfWork);
                    var ConfigurationEntity = ConfigurationRepository.Get(e => e.Id != null);

                    Settings Settings = AutoMapper.Mapper.Map<Entities.User, Settings>(UserEntity);
                    return AutoMapper.Mapper.Map<Entities.Configuration, Settings>(ConfigurationEntity, Settings);
                }
            }
        }
        #endregion

        #region Administration
        public User SetLocalCredentialState(Guid UserId, string State)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    UserEntity.SetLocalCredentialState(State);
                    var SavedUser = UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return AutoMapper.Mapper.Map<Entities.User, User>(SavedUser);
                }
            }
        }

        public User SetExternalCredentialState(Guid UserId, string State)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    UserEntity.SetExternalCredentialState(State);
                    var SavedUser = UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return AutoMapper.Mapper.Map<Entities.User, User>(SavedUser);
                }
            }
        }

        //public User Update(User User)
        //{
        //    using (new FunctionLogger(Log))
        //    {
        //        using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
        //        {
        //            var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
        //            var UserEntity = UserRepository.Get(User.Id);

        //            UserEntity = AutoMapper.Mapper.Map<User, Entities.User>(User, UserEntity);

        //            if (User.LocalCredentialState != UserEntity.LocalCredentialState)
        //                UserEntity.SetLocalCredentialState(User.LocalCredentialState);

        //            if (User.ExternalCredentialState != UserEntity.ExternalCredentialState)
        //                UserEntity.SetExternalCredentialState(User.ExternalCredentialState);

        //            UserEntity = UserRepository.Update(UserEntity);
        //            UnitOfWork.Commit();

        //            return AutoMapper.Mapper.Map<Entities.User, User>(UserEntity);
        //        }
        //    }
        //}
        #endregion

        #region Roles and Activities
        public IList<Role> GetRoles(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = Repository.Get(e => e.Id == UserId, "Roles");

                    IList<Role> Roles = new List<Role>();
                    Roles = AutoMapper.Mapper.Map<IList<Entities.Role>, IList<Dtos.Role>>(UserEntity.Roles, Roles);

                    return Roles;
                }
            }
        }

        public void SetRoles(Guid UserId, IList<Role> Roles)
        {
            using (new FunctionLogger(Log))
            {
                var RoleIds =
                    from Role R in Roles
                    where R.IsNew == false
                    select R.Id;

                if (RoleIds.Count() != Roles.Count())
                    throw new ArgumentException("User.Roles");

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var RoleRepository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                    var ExistingRoles = RoleRepository.GetAsIQueryable(e => e.Id != Guid.Empty, null, "Activities").Where(e => RoleIds.Contains(e.Id));

                    // Now read, update and save the user entity with the roles. When we read in the user, we need to eagerly
                    // read the roles as well - otherwise, when we write back to the database, EF will see the roles as inserts
                    // only, not as a change to the roles the user is in
                    var UserEntity = UserRepository.Get(e => e.Id == UserId, "Roles");

                    UserEntity.Roles = ExistingRoles.ToList();

                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();
                }
            }
        }

        public IList<Activity> GetActivities(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(e => e.Id == UserId, "Roles, Roles.Activities");

                    IList<Entities.Activity> ActivityEntities = new List<Entities.Activity>();

                    foreach (Entities.Role RoleEntity in UserEntity.Roles)
                    {
                        foreach (Entities.Activity ActivityEntity in RoleEntity.Activities)
                        {
                            if (!ActivityEntities.Contains(ActivityEntity))
                            {
                                ActivityEntities.Add(ActivityEntity);
                            }
                        }
                    }

                    IList<Dtos.Activity> ActivityDtos = new List<Dtos.Activity>();
                    ActivityDtos = AutoMapper.Mapper.Map<IList<Entities.Activity>, IList<Dtos.Activity>>(ActivityEntities, ActivityDtos);

                    return ActivityDtos;
                }
            }
        }

        public bool UserHasActivity(Guid UserId, string ActivityName)
        {
            using (new FunctionLogger(Log))
            {
                if (PermissionCache.ContainsKey(UserId) == false)
                {
                    var Activities = GetActivities(UserId);
                    PermissionCache[UserId] = Activities;
                }

                if (PermissionCache[UserId].FirstOrDefault(e => e.Name == ActivityName) == null)
                    return false;

                return true;
            }
        }
        #endregion
    }
}
