using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ConfirmationModel : Model<ConfirmationModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid ConfirmationId { get; set; }
        #endregion

        public ConfirmationModel() { }
        public ConfirmationModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: false)
        { }

        public override bool Write(ConfirmationModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
                var ConfirmType = ConfirmationService.GetConfirmType(Model.ConfirmationId);

                if (ConfirmType == "Register Local Account")
                {
                    var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
                    var User = AuthenticationService.CompleteRegistration(Model.ConfirmationId);

                    AuthenticationAPI.SignIn(User.Id, User.Fullname.DisplayName, false);

                    FeedbackAPI.DisplaySuccess("Thank you for confirming your email address and completing your registration - welcome " + User.Fullname.Firstname + ".");
                    return true;
                }

                if (ConfirmType == "Change Email Address")
                {
                    var UserService = ServiceFactory.CreateService<IUserService>();

                    UserService.CompleteChangeEmailAddress(Model.ConfirmationId);
                    FeedbackAPI.DisplaySuccess("The email address for this account has been successfully changed.");

                    return true;
                }

                if (ConfirmType == "Register Local Credentials")
                {
                    var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
                    var User = AuthenticationService.CompleteSetLocalCredentials(Model.ConfirmationId);

                    AuthenticationAPI.SignOut();
                    AuthenticationAPI.SignIn(User.Id, User.Fullname.DisplayName, false);
                    AuthenticationAPI.LoggedInCredentialState = User.LocalCredentialState;
                    FeedbackAPI.DisplaySuccess("Your local credentials have been confirmed.");

                    return true;
                }

                throw new NotImplementedException("An unexpected error has occurred. The confirmation has not been successful.");
            }
        }
    }
}
