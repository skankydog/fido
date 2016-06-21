using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class RegistrationInitiate : Model<RegistrationInitiate>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        [Required]
        [EmailAddress]
        [Display(Name = "email address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "first name")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "last name")]
        public string Surname { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }
        #endregion

      //  public RegistrationInitiate() { }
        public RegistrationInitiate()
            //IFeedbackAPI FeedbackAPI,
            //IAuthenticationAPI LoginAPI,
            //IModelAPI ModelAPI)
                : base (//FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: false, RequiresWritePermission: false)
        { }

        public override bool Save(RegistrationInitiate Model)
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

                var ConfirmationId = AuthenticationService.RegistrationInitiate(Model.EmailAddress, Model.Password, Model.Firstname, Model.Surname);

                //PresentIL.SendConfirmation(Yikes.CompleteForgottenLocalPassword, UserInterface.UserId, ConfirmationId);
                //if (UserInterface.Mode != Mode.Production)
                //{
                //    var ConfirmationLink = Url.Action("CompleteForgottenLocalPassword") + "?" + ConfirmationId.ToString();
                //    UserInterface.DisplayInfo("For development use only, click the below link to simulate response to the email.",
                //        new FlashLinks { Links = new List<string> { ConfirmationLink } });
                //}
                //else
                FeedbackAPI.DisplaySuccess("An email will shortly be sent to your nominated email address for confirmation - once confirmed, you will be able to login.");

                return true;
            }
        }
    }
}
