using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ExternalLoginCallback : Model<ExternalLoginCallback>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        #endregion

        public ExternalLoginCallback() { }
        public ExternalLoginCallback(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: false, RequiresWritePermission: false)
        { }

        public override bool Save(ExternalLoginCallback Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
                var User = AuthenticationService.LoginByExternalCredentials(Model.LoginProvider, Model.ProviderKey);

                if (User != null)
                {
                    AuthenticationAPI.SignIn(User.Id, User.Fullname.FirstnameSurname, false);
                    AuthenticationAPI.LoggedInCredentialState = User.ExternalCredentialState;
                    
                    return true;
                }

                User = AuthenticationService.LoginByExternalEmailAddress(Model.LoginProvider, Model.ProviderKey, Model.EmailAddress);

                if (User != null)
                {
                    AuthenticationAPI.SignIn(User.Id, User.Fullname.FirstnameSurname, false);
                    AuthenticationAPI.LoggedInCredentialState = User.ExternalCredentialState;

                    FeedbackAPI.DisplaySuccess("The external credentials have been linked to an already existing account via your email address");
                    return true;
                }

                User = AuthenticationService.CreateByExternalCredentials(Model.LoginProvider, Model.ProviderKey, Model.EmailAddress, Model.Name);
                
                if (User != null)
                {
                    AuthenticationAPI.SignIn(User.Id, User.Fullname.FirstnameSurname, false);
                    AuthenticationAPI.LoggedInCredentialState = User.ExternalCredentialState;

                    FeedbackAPI.DisplaySuccess("A new account has been created");
                    return true;
                }

                FeedbackAPI.DisplayError("Unable to find or create a user record");
                return false;                
            }
        }
    }
}
