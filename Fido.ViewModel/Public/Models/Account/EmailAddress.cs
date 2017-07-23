using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.ViewModel.Implementation;

namespace Fido.ViewModel.Models.Account
{
    public class EmailAddress : Model<EmailAddress>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        [Required]
        [EmailAddress]
        [Display(Name = "email address")]
        public string Email { get; set; }

        public Guid ConfirmationId { get; set; }
        #endregion

        public EmailAddress()
            : base(ReadAccess: Access.Authenticated, WriteAccess: Access.Authenticated)
        { }

        public override bool Write(EmailAddress Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                if (!AuthenticationService.EmailAddressIsFree(Model.Email))
                {
                    ModelAPI.PropertyError("EmailAddress", "The email address is already in use");
                    return false;
                }

                if (!AuthenticationService.EmailAddressPassesValidation(Model.Email))
                {
                    ModelAPI.PropertyError("EmailAddress", "The email address is not of a valid format");
                    return false;
                }

                var UserService = ServiceFactory.CreateService<IUserService>();

                ConfirmationId = UserService.ChangeEmailAddressInitiate(AuthenticationAPI.AuthenticatedId, Model.Email); // TEST MODE??

                //if (System.Configuration.ConfigurationManager.AppSettings["UI-Mode"] == "Development" ||
                //    System.Configuration.ConfigurationManager.AppSettings["UI-Mode"] == "Test")
                //{
                //    var ConfirmationLink = Url.Action("CompleteForgottenLocalPassword") + "?" + ConfirmationId.ToString();
                //    UserInterface.DisplayInfo("For development use only, click the below link to simulate response to the email.",
                //        new FlashLinks { Links = new List<string> { ConfirmationLink } });
                //}
                //else
                FeedbackAPI.DisplaySuccess("An email will shortly be sent to " + Model.Email + " - once confirmed, your credentials will be updated");

                return true;
            }
        }

        public override bool Confirm(Guid ConfirmationId)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                UserService.ChangeEmailAddressComplete(ConfirmationId);
                FeedbackAPI.DisplaySuccess("The email address for this account has been successfully changed.");

                return true;
            }
        }
    }
}
