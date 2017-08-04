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
    internal class AuthenticationService : IAuthenticationService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<Guid, IList<Activity>> PermissionCache = new Dictionary<Guid, IList<Activity>>(); // TO DO: not necessary

        #region Has Local/External Credentials
        public bool HasLocalCredentials(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = Repository.Get(UserId);

                    return UserEntity.CurrentLocalCredentialState.ArePresent;
                }
            }
        }

        public bool HasExternalCredentials(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = Repository.Get(UserId);

                    return UserEntity.CurrentExternalCredentialState.ArePresent;
                }
            }
        }
        #endregion

        #region Login
        public User LoginByLocalCredentials(string EmailAddress, string Password)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(e => e.EmailAddress == EmailAddress && e.Password == Password);

                    if (UserEntity == null)
                        return null;

                    UserEntity.CurrentLocalCredentialState.Login();
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }

        public User LoginByExternalCredentials(string LoginProvider, string ProviderKey)
        {
            using (new FunctionLogger(Log))
            {
                Log.DebugFormat("LoginProvider: {0}", LoginProvider);
                Log.DebugFormat("ProviderKey: {0}", ProviderKey);

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.GetByExternalCredentials(LoginProvider, ProviderKey);

                    if (UserEntity == null)
                        return null;

                    UserEntity.CurrentExternalCredentialState.Login();
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }

        public User LoginByExternalEmailAddress(string LoginProvider, string ProviderKey, string EmailAddress)
        {
            // This function uses the email address to look for a matching entry either in the user entity (local
            // credentials) or the external credentials. If it finds on either, the passed in external credentials
            // are used to create a new external credential and the user is passed back.
            //
            // The idea behind this function is to try and match external credentials to already-registered users
            // in the system.
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Login Provider: {0}", LoginProvider);
                Log.InfoFormat("Provider Key: {0}", ProviderKey);
                Log.InfoFormat("Email Address: {0}", EmailAddress);

                if (EmailAddress == null)
                {
                    Log.Info("External credentials do not contain an email address - returning");
                    return null;
                }

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccess.DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(e => e.EmailAddress == EmailAddress);

                    if (UserEntity == null)
                    {
                        UserEntity = UserRepository.GetByExternalEmailAddress(EmailAddress);

                        if (UserEntity == null)
                        {
                            Log.InfoFormat("Email address, {0}, not found on user entities or any external credentials", EmailAddress);
                            return null;
                        }
                    }

                    UserEntity.CurrentExternalCredentialState.Link(LoginProvider, ProviderKey, EmailAddress);
                    UserEntity.CurrentExternalCredentialState.Login();
                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }

        public User CreateByExternalCredentials(string LoginProvider, string ProviderKey, string EmailAddress, string Name)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Login Provider: {0}", LoginProvider);
                Log.InfoFormat("Provider Key: {0}", ProviderKey);
                Log.InfoFormat("Email Address: {0}", EmailAddress);
                Log.InfoFormat("Name: {0}", Name);

                if (EmailAddress == null) { EmailAddress = string.Concat("none.", Guid.NewGuid()); }

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccess.DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.GetByExternalCredentials(LoginProvider, ProviderKey);

                    if (UserEntity != null)
                        throw new Exception("External credentials already in use");

                    UserEntity = new Entities.User();

                    UserEntity.CurrentExternalCredentialState.Register(EmailAddress, Name);
                    UserEntity.CurrentExternalCredentialState.Link(LoginProvider, ProviderKey, EmailAddress);

                    UserRepository.DeepInsert(UserEntity);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }
        #endregion

        #region Registration
        private string REGISTRATION = "Register Local Account";

        public Guid RegistrationInitiate(string EmailAddress, string Password, string Firstname, string Surname, bool AssumeSent)
        {
            using (new FunctionLogger(Log))
            {
                if (EmailAddressIsFree(EmailAddress) == false)
                    throw new EmailAddressDuplicationException(EmailAddress);

                if (EmailAddressPassesValidation(EmailAddress) == false)
                    throw new EmailAddressValidationException(EmailAddress);

                if (PasswordPassesValidation(Password) == false)
                    throw new PasswordValidationException();

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                    var UserEntity = new Entities.User()
                    {
                        Id = Guid.NewGuid(),
                        CreatedUtc = DateTime.UtcNow
                    };

                    Guid ConfirmationId = ConfirmationService.QueueConfirmation(UnitOfWork, REGISTRATION, UserEntity.Id, EmailAddress, AssumeSent);
                    UserEntity.CurrentLocalCredentialState.InitiateRegistration(EmailAddress, Password, Firstname, Surname);

                    UserRepository.Insert(UserEntity);
                    UnitOfWork.Commit();

                    return ConfirmationId;
                }
            }
        }

        public User RegistrationComplete(Guid ConfirmationId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var Confirmation = ConfirmationService.ReceiveConfirmation(UnitOfWork, ConfirmationId, REGISTRATION);

                    if (Confirmation == null)
                        throw new ServiceException(string.Format("Unable to complete registration - {0} not found or expired", ConfirmationId));

                    var UserEntity = UserRepository.Get(Confirmation.UserId);

                    UserEntity.CurrentLocalCredentialState.CompleteRegistration();
                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }
        #endregion

        #region Set Local Credential
        private string SET_LOCAL_CREDENTIAL = "Register Local Credential";

        public Guid SetLocalCredentialInitiate(Guid UserId, string EmailAddress, string Password, bool AssumeSent)
        {
            using (new FunctionLogger(Log))
            {
                if (EmailAddressIsFree(EmailAddress) == false)
                    throw new EmailAddressDuplicationException(EmailAddress);

                if (EmailAddressPassesValidation(EmailAddress) == false)
                    throw new EmailAddressValidationException(EmailAddress);

                if (PasswordAdvisor.WeakOrInvalid(Password))
                    throw new PasswordValidationException();

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    Guid ConfirmationId = ConfirmationService.QueueConfirmation(UnitOfWork, SET_LOCAL_CREDENTIAL, UserEntity.Id, EmailAddress, AssumeSent);

                    UserEntity.CurrentLocalCredentialState.InitiateSetLocalCredentials(EmailAddress, Password);
                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return ConfirmationId;
                }
            }
        }

        public User SetLocalCredentialComplete(Guid ConfirmationId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var Confirmation = ConfirmationService.ReceiveConfirmation(UnitOfWork, ConfirmationId, SET_LOCAL_CREDENTIAL);

                    if (Confirmation == null)
                        throw new ServiceException(string.Format("Unable to complete setting of credentials - {0} not found or expired", ConfirmationId));

                    var UserEntity = UserRepository.Get(Confirmation.UserId);

                    UserEntity.CurrentLocalCredentialState.CompleteSetLocalCredentials();
                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }
        #endregion

        #region Forgotten Password
        private string FORGOTTEN_PASSWORD = "Forgotten Password";

        public Guid ForgottenPasswordInitiate(string EmailAddress, bool AssumeSent)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(e => e.EmailAddress == EmailAddress);

                    if (UserEntity == null)
                        throw new Exception(string.Format("User with an email address of {0} not found", EmailAddress));

                    Guid ConfirmationId = ConfirmationService.QueueConfirmation(UnitOfWork, FORGOTTEN_PASSWORD, UserEntity.Id, UserEntity.EmailAddress, AssumeSent);
                    UserEntity.CurrentLocalCredentialState.InitiateForgottenPassword();

                    UnitOfWork.Commit();
                    return ConfirmationId;
                }
            }
        }

        public Confirmation ForgottenPasswordReceive(Guid ConfirmationId)
        {
            using (new FunctionLogger(Log))
            {
                Entities.Confirmation ConfirmationEntity;

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    ConfirmationEntity = ConfirmationService.ReceiveConfirmation(UnitOfWork, ConfirmationId, FORGOTTEN_PASSWORD);

                    if (ConfirmationEntity == null)
                        throw new ServiceException(string.Format("Unable to change forgotten password - {0} not found or expired", ConfirmationId));
                }

                return Mapper.Map<Entities.Confirmation, Dtos.Confirmation>(ConfirmationEntity);
            }
        }

        public User ForgottenPasswordComplete(Guid UserId, string Password)
        {
            using (new FunctionLogger(Log))
            {
                if (PasswordAdvisor.WeakOrInvalid(Password))
                    throw new PasswordValidationException();

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    //DateTime CurrentDateTime = DateTime.UtcNow;
                    //TimeSpan TimeSinceForgotten = CurrentDateTime.Subtract(UserEntity.Authentication.PasswordForgottenUtc);

                    //if (TimeSinceForgotten.TotalMinutes > 30) // To do: Put this in configuration
                    //    throw new ServiceException("Too long since the forgotten password link was sent");

                    UserEntity.CurrentLocalCredentialState.CompleteForgottenPassword(Password);
                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();

                    return Mapper.Map<Entities.User, Dtos.User>(UserEntity);
                }
            }
        }
        #endregion

        #region Password Logic
        public bool PasswordPassesValidation(string Password)
        {
            using (new FunctionLogger(Log))
            {
                return PasswordAdvisor.MediumOrHigher(Password);
            }
        }

        public PasswordScore GetPasswordScore(string Password)
        {
            return PasswordAdvisor.CheckStrength(Password);
        }
        #endregion

        #region Email Address Logic
        public bool EmailAddressIsFree(string EmailAddress)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                if (UserService.GetByEmailAddress(EmailAddress) == null)
                    return true;

                return false;
            }
        }

        public bool EmailAddressPassesValidation(string EmailAddress)
        {
            using (new FunctionLogger(Log))
            {
                string Match = "^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$";
                System.Text.RegularExpressions.Regex EmailAddressRegEx = new System.Text.RegularExpressions.Regex(Match);

                if (EmailAddressRegEx.Matches(EmailAddress).Count > 0)
                    return true;

                return false;
            }
        }
        #endregion

        #region Manage External Credentials
        public IList<ExternalCredential> GetExternalCredentials(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = Repository.Get(e => e.Id == UserId, "ExternalCredentials");

                    IList<ExternalCredential> ExternalCredentials = new List<ExternalCredential>();
                    return Mapper.Map<IList<Entities.ExternalCredential>, IList<Dtos.ExternalCredential>>(UserEntity.ExternalCredentials, ExternalCredentials);
                }
            }
        }

        public void LinkExternalCredentials(Guid UserId, string LoginProvider, string ProviderKey, string EmailAddress)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("User Id: {0}", UserId);
                Log.InfoFormat("Login Provider: {0}", LoginProvider);
                Log.InfoFormat("Provider Key: {0}", ProviderKey);
                Log.InfoFormat("Email Address: {0}", EmailAddress);

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccess.DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    //var UserEntity = UserRepository.Get(UserId, "ExternalCredentials");
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new Exception("User not found");

                    var ExternalCredentialRepository = DataAccess.DataAccessFactory.CreateRepository<IExternalCredentialRepository>(UnitOfWork);
                    var ExternalCredentialEntity = ExternalCredentialRepository.Get(e => e.LoginProvider == LoginProvider && e.ProviderKey == ProviderKey);

                    if (ExternalCredentialEntity != null)
                        throw new Exception("External credentials already used");

                    UserEntity.CurrentExternalCredentialState.Link(LoginProvider, ProviderKey, EmailAddress);
                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();
                }
            }
        }

        public void UnlinkExternalCredentials(Guid UserId, Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("User Id: {0}", UserId);
                Log.InfoFormat("External Credential Id: {0}", Id);

                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccess.DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                    var UserEntity = UserRepository.Get(UserId);

                    if (UserEntity == null)
                        throw new Exception("User not found");

                    UserEntity.CurrentExternalCredentialState.Unlink(Id);
                    UserRepository.Update(UserEntity);
                    UnitOfWork.Commit();
                }
            }
        }
        #endregion
    }
}
