using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ForgottenPasswordInitiate : Model<ForgottenPasswordInitiate>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        [Required]
        [EmailAddress]
        [Display(Name = "email address")]
        public string EmailAddress { get; set; }
        #endregion

 //       public ForgottenPasswordInitiate() { }
        public ForgottenPasswordInitiate()
            //IFeedbackAPI FeedbackAPI,
            //IAuthenticationAPI LoginAPI,
            //IModelAPI ModelAPI)
                : base (//FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: false, RequiresWritePermission: false)
        { }

        public override bool Save(ForgottenPasswordInitiate Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                if (!AuthenticationService.EmailAddressPassesValidation(Model.EmailAddress))
                {
                    ModelAPI.PropertyError("EmailAddress", "The email address is not of a valid format");
                    return false;
                }

                var ConfirmationId = AuthenticationService.ForgottenPasswordInitiate(Model.EmailAddress);

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
