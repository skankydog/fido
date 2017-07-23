using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;
using Fido.Core;
using Fido.Dtos;
using Fido.DataAccess;
using Fido.Service.Exceptions;

namespace Fido.Service.Implementation
{
    internal class UserService : CRUDService<User, Entities.User, DataAccess.IUserRepository>, IUserService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Pages
        public IList<User> GetPageInDefaultOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Id),
                OrderByDescending: q => q.OrderByDescending(s => s.Id));
        }

        public IList<User> GetPageInFirstnameOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Fullname.Firstname),
                OrderByDescending: q => q.OrderByDescending(s => s.Fullname.Firstname));
        }

        public IList<User> GetPageInSurnameOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Fullname.Surname),
                OrderByDescending: q => q.OrderByDescending(s => s.Fullname.Surname));
        }

        public IList<User> GetPageInEmailAddressOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.EmailAddress),
                OrderByDescending: q => q.OrderByDescending(s => s.EmailAddress));
        }

        public IList<User> GetPageInLocalCredentialOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.LocalCredentialState),
                OrderByDescending: q => q.OrderByDescending(s => s.LocalCredentialState));
        }

        public IList<User> GetPageInExternalCredentialOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.ExternalCredentialState),
                OrderByDescending: q => q.OrderByDescending(s => s.ExternalCredentialState));
        }

        private IList<User> GetPage(char SortOrder, int Skip, int Take, string Filter,
            Func<IQueryable<Entities.User>, IOrderedQueryable<Entities.User>> OrderByAscending,
            Func<IQueryable<Entities.User>, IOrderedQueryable<Entities.User>> OrderByDescending)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var OrderBy = SortOrder == 'a' ? OrderByAscending : OrderByDescending;
                    var Query = UserRepository.GetAsIQueryable(e => e.Id != null, OrderBy);

                    if (Filter.IsNotNullOrEmpty())
                    {
                        Query = Query.Where(e => e.Fullname.Firstname.ToLower().Contains(Filter.ToLower())
                                              || e.Fullname.Surname.ToLower().Contains(Filter.ToLower())
                                              || e.EmailAddress.ToLower().Contains(Filter.ToLower())
                                              || e.LocalCredentialState.ToLower().Contains(Filter.ToLower())
                                              || e.ExternalCredentialState.ToLower().Contains(Filter.ToLower()));
                    }

                    Query = Query.Skip(Skip).Take(Take);
                    
                    var EntityList = Query.ToList(); // Hit the database

                    IList<User> DtoList = AutoMapper.Mapper.Map<IList<Entities.User>, IList<User>>(EntityList);
                    return DtoList;
                }
            }
        }
        #endregion

        #region Change Email Address
        private const string CHANGE_EMAIL_ADDRESS = "Change Email Address";

        public Guid ChangeEmailAddressInitiate(Guid UserId, string NewEmailAddress, bool AssumeSent)
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

                    Guid ConfirmationId = ConfirmationService.QueueConfirmation(UnitOfWork, CHANGE_EMAIL_ADDRESS, UserId, NewEmailAddress, AssumeSent);
                    UserEntity.CurrentLocalCredentialState.InitiateChangeEmailAddress();

                    UnitOfWork.Commit();
                    return ConfirmationId;
                }
            }
        }

        public User ChangeEmailAddressComplete(Guid ConfirmationId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var Confirmation = ConfirmationService.ReceiveConfirmation(UnitOfWork, ConfirmationId, CHANGE_EMAIL_ADDRESS);

                    if (Confirmation == null)
                    {
                        throw new ServiceException(string.Format("Unable to change email address - {0} not found or expired", ConfirmationId));
                    }

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
                        throw new ServiceException(string.Format("User id, {0}, does not exist or incorrect password has been entered", UserId));

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
                    var UserEntity = Repository.Get(e => e.EmailAddress == EmailAddress);

                    User UserDTO = AutoMapper.Mapper.Map<Entities.User, User>(UserEntity);

                    return UserDTO;
                }
            }
        }
        #endregion

        #region Profile
        // Interestingly, found an issue when reading the user entity and including the image. The image entity was
        // not resolving. Turns out it appears to be an issue in the .NET layer and you need to revert to the 2010
        // one; http://blogs.msdn.com/b/visualstudioalm/archive/2013/10/16/switching-to-managed-compatibility-mode-in-visual-studio-2013.aspx
        //
        // I suspect I will need to utilise the old EXE interpreter as well when I build the release version.
        public Profile GetProfile(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId, "UserImage");

                    var ProfileDto = AutoMapper.Mapper.Map<Entities.User, Profile>(UserEntity);

                    return ProfileDto;
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
                    var UserEntity = UserRepository.Get(Profile.Id, "Roles, ExternalCredentials, UserImage");

                    UserEntity = AutoMapper.Mapper.Map<Profile, Entities.User>(Profile, UserEntity);

                    UserRepository.Update(UserEntity);
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
                    var UserEntity = UserRepository.Get(UserId);

                    var ConfigurationRepository = DataAccessFactory.CreateRepository<IConfigurationRepository>(UnitOfWork);
                    var ConfigurationEntity = ConfigurationRepository.Get(e => e.Id != null);

                    Settings Settings = AutoMapper.Mapper.Map<Entities.User, Settings>(UserEntity);
                    return AutoMapper.Mapper.Map<Entities.Configuration, Settings>(ConfigurationEntity, Settings);
                }
            }
        }
        #endregion

        #region Administration
        public User CreateAsAdministrator(Guid UserId, string Firstname, string Surname, string EmailAddress, string Password)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = new Entities.User
                            {
                                Id = UserId,
                                LocalCredentialState = Entities.UserDetails.LocalCredentialStates.Expired.Name,
                                Fullname = new Entities.UserDetails.Fullname
                                    {
                                        Firstname = Firstname,
                                        Surname = Surname
                                    },
                                EmailAddress = EmailAddress,
                                Password = Password
                            };

                    var SavedUser = UserRepository.Insert(UserEntity);
                    UnitOfWork.Commit();

                    return AutoMapper.Mapper.Map<Entities.User, User>(SavedUser);
                }
            }
        }

        public User UpdateAsAdministrator(User User)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(User.Id);

                    UserEntity = AutoMapper.Mapper.Map<User, Entities.User>(User, UserEntity);

                    UserEntity.LocalCredentialState = User.LocalCredentialState; // Not automapped
                    UserEntity.ExternalCredentialState = User.ExternalCredentialState; // Not automapped
//                    UserEntity.EmailAddress = User.EmailAddress != null ? User.EmailAddress : UserEntity.EmailAddress; // Not automapped

                    var SavedUser = UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return AutoMapper.Mapper.Map<Entities.User, User>(SavedUser);
                }
            }
        }

        public User ResetLocalCredentialsAsAdministrator(Guid UserId, string EmailAddress, string Password)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    UserEntity.LocalCredentialState = Entities.UserDetails.LocalCredentialStates.Expired.Name;
                    UserEntity.EmailAddress = EmailAddress;
                    UserEntity.Password = Password;

                    var SavedUser = UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return AutoMapper.Mapper.Map<Entities.User, User>(SavedUser);
                }
            }
        }
        #endregion

        #region Roles and Activities
        public IList<Role> GetRoles(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = Repository.Get(e => e.Id == UserId);

                    IList<Role> Roles = new List<Role>();
                    Roles = AutoMapper.Mapper.Map<ICollection<Entities.Role>, IList<Dtos.Role>>(UserEntity.Roles, Roles);

                    return Roles;
                }
            }
        }

        public User SetRoles(Guid UserId, IList<Role> Roles)
        {
            using (new FunctionLogger(Log))
            {
                var RoleEntities = AutoMapper.Mapper.Map<IList<Role>, IList<Entities.Role>>(Roles);

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(e => e.Id == UserId);

                    UserEntity.Roles = RoleEntities;

                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return AutoMapper.Mapper.Map<Entities.User, User>(UserEntity);
                }
            }
        }

        //public void SetRoles(Guid UserId, IList<Role> Roles)
        //{
        //    using (new FunctionLogger(Log))
        //    {
        //        var RoleIds =
        //            from Role R in Roles
        //            where R.IsNew == false
        //            select R.Id;

        //        if (RoleIds.Count() != Roles.Count())
        //            throw new ArgumentException("Setting new roles directly to a user is not supported");

        //        using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
        //        {
        //            var RoleRepository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
        //            var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

        //            var RolesFromDb = RoleRepository.GetAsIQueryable(e => e.Id != Guid.Empty, null, "Activities").Where(e => RoleIds.Contains(e.Id));

        //            // Now read, update and save the user entity with the roles. When we read in the user, we need to eagerly
        //            // read the roles as well - otherwise, when we write back to the database, EF will see the roles as inserts
        //            // only, not as a change to the roles the user is in
        //            var UserEntity = UserRepository.Get(e => e.Id == UserId, "Roles");

        //            UserEntity.Roles = RolesFromDb.ToList();

        //            UserRepository.Update(UserEntity); // DeepUpdate()?!!
        //            UnitOfWork.Commit();
        //        }
        //    }
        //}

        //public IList<Activity> GetAllowedActivities(Guid UserId)
        //{
        //    using (new FunctionLogger(Log))
        //    {
        //        var ActivityEntities = GetActivities(UserId);

        //        IList<Dtos.Activity> ActivityDtos = new List<Dtos.Activity>();
        //        ActivityDtos = AutoMapper.Mapper.Map<IList<Entities.Activity>, IList<Dtos.Activity>>(ActivityEntities, ActivityDtos);

        //        return ActivityDtos;
        //    }
        //}

        public IList<Activity> GetDeniedActivities(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var ActivityRepository = DataAccessFactory.CreateRepository<IActivityRepository>(UnitOfWork);

                    var All = ActivityRepository.GetAsIEnumerable(a => a.Id != null);
                    var Allowed = GetActivities(UserId);
                    var Denied = All.Where(a => !Allowed.Any(u => a.Id == u.Id)).ToList();

                    IList<Dtos.Activity> ActivityDtos = new List<Dtos.Activity>();
                    ActivityDtos = AutoMapper.Mapper.Map<IList<Entities.Activity>, IList<Dtos.Activity>>(Denied, ActivityDtos);

                    return ActivityDtos;
                }
            }
        }

        private IList<Entities.Activity> GetActivities(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(e => e.Id == UserId, "Roles.Activities");

                    if (UserEntity == null)
                        return new List<Entities.Activity>(); // no entities for an invalid user id

                    var ActivityEntities = new List<Entities.Activity>();

                    foreach (var RoleEntity in UserEntity.Roles)
                    {
                        foreach (var ActivityEntity in RoleEntity.Activities)
                            ActivityEntities.Add(ActivityEntity);
                    }

                    ActivityEntities = ActivityEntities.DistinctBy(e => e.Id).ToList();
                    return ActivityEntities;
                }
            }
        }

        public bool UserHasActivity(Guid UserId, string Name, string Area, string Action)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<DataAccess.IUserRepository>(UnitOfWork);
                    var Activities = (from u in UserRepository.GetAsIQueryable(e => e.Id == UserId)
                                      from r in u.Roles
                                      from a in r.Activities
                                      where a.Name == Name && a.Area == Area && a.ReadWrite == Action
                                      select u).DistinctBy(u => u.Id).Count();

                    return Activities == 1;
                }
            }
        }
        #endregion
    }
}
