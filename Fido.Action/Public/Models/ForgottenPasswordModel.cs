using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ForgottenPasswordModel : Model<ForgottenPasswordModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        [Required]
        [EmailAddress]
        [Display(Name = "email address")]
        public string EmailAddress { get; set; }
        #endregion

        public ForgottenPasswordModel() { } // pure model
        public ForgottenPasswordModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: false)
        { }

        public override bool Write(ForgottenPasswordModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                var User = UserService.GetByEmailAddress(Model.EmailAddress);

                if (User == null || User.LocalCredentialState != "Active")
                {
                    ModelAPI.ModelError("The user account either does not exist or the email address is not yet confirmed.");
                    return false;
                }

                var ConfirmationId = AuthenticationService.InitiateForgottenPassword(User.EmailAddress);

                //if (System.Configuration.ConfigurationManager.AppSettings["UI-Mode"] == "Development" ||
                //    System.Configuration.ConfigurationManager.AppSettings["UI-Mode"] == "Test")
                //{
                //    var ConfirmationLink = Url.Action("CompleteForgottenLocalPassword") + "?ConfirmationId=" + ConfirmationId.ToString();
                //    Flash.Info("For development use only, click the below link to simulate response to the email.",
                //        new FlashLinks { Links = new List<string> { ConfirmationLink } });
                //}
                //else

                FeedbackAPI.DisplaySuccess("An email will shortly be sent to your nominated email address for confirmation - follow the link within to reset your password");
                return true;
            }
        }
    }
}
