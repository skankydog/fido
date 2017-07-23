using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.ViewModel.Implementation;

namespace Fido.ViewModel.Models.Account
{
    public class LocalCredential : Model<LocalCredential>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
  //      public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public Guid ConfirmationId { get; set; }
        #endregion

        public LocalCredential()
            : base(ReadAccess: Access.Authenticated, WriteAccess: Access.Authenticated)
        { }

        public override bool Write(LocalCredential Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                if (!AuthenticationService.EmailAddressIsFree(Model.EmailAddress))
                {
                    ModelAPI.PropertyError("EmailAddress", "The email address is already in use");
                    return false;
                }

                if (!AuthenticationService.EmailAddressPassesValidation(Model.EmailAddress))
                {
                    ModelAPI.PropertyError("EmailAddress", "The email address is not of a valid format");
                    return false;
                }

                if (!AuthenticationService.PasswordPassesValidation(Model.Password))
                {
                    ModelAPI.PropertyError("Password", "The password does not meet the minimum validation requirements");
                    return false;
                }

                ConfirmationId = AuthenticationService.SetLocalCredentialInitiate(AuthenticationAPI.AuthenticatedId, Model.EmailAddress, Model.Password);

                //if (System.Configuration.ConfigurationManager.AppSettings["UI-Mode"] == "Development" ||
                //    System.Configuration.ConfigurationManager.AppSettings["UI-Mode"] == "Test")
                //{
                //    var ConfirmationLink = Url.Action("CompleteForgottenLocalPassword") + "?" + ConfirmationId.ToString();
                //    UserInterface.DisplayInfo("For development use only, click the below link to simulate response to the email.",
                //        new FlashLinks { Links = new List<string> { ConfirmationLink } });
                //}
                //else
                FeedbackAPI.DisplaySuccess("An email will shortly be sent to " + Model.EmailAddress + " - once confirmed, your credentials will be set");

                return true;
            }
        }

        public override bool Confirm(Guid ConfirmationId)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
                var User = AuthenticationService.SetLocalCredentialComplete(ConfirmationId);

                AuthenticationAPI.SignOut();
                AuthenticationAPI.SignIn(User.Id, User.Fullname.FirstnameSurname, false);
                AuthenticationAPI.LoggedInCredentialState = User.LocalCredentialState;
                FeedbackAPI.DisplaySuccess("Your local credentials have been confirmed.");

                return true;
            }
        }
    }
}
