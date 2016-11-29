using AutoMapper;
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
    internal class AdministrationService : IAdministrationService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public User EnableLocalCredentials(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new Exception(string.Format("User with an id of {0} not found", UserId));

                    UserEntity.LocalCredentialState = Fido.Entities.UserDetails.LocalCredentialStates.Enabled.Name;

                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }

        public User DisableLocalCredentials(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new Exception(string.Format("User with an id of {0} not found", UserId));

                    UserEntity.LocalCredentialState = Fido.Entities.UserDetails.LocalCredentialStates.Disabled.Name;

                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }

        public User SetEmailAddress(Guid UserId, string EmailAddress)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new Exception(string.Format("User with an id of {0} not found", UserId));

                    UserEntity.CurrentLocalCredentialState.SetEmailAddress(EmailAddress);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }

        public User SetLocalPassword(Guid UserId, string Password)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new Exception(string.Format("User with an id of {0} not found", UserId));

                    UserEntity.CurrentLocalCredentialState.SetPassword(Password);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }

        public User ClearLocalCredentials(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new Exception(string.Format("User with an id of {0} not found", UserId));

                    UserEntity.CurrentLocalCredentialState.Clear();
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }

        public User EnableExternalCredentials(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new Exception(string.Format("User with an id of {0} not found", UserId));

                    UserEntity.ExternalCredentialState = Fido.Entities.UserDetails.ExternalCredentialStates.Enabled.Name;

                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }
        
        public User DisableExternalCredentials(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new Exception(string.Format("User with an id of {0} not found", UserId));

                    UserEntity.ExternalCredentialState = Fido.Entities.UserDetails.ExternalCredentialStates.Disabled.Name;

                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }
    }
}
