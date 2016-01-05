using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class SetCredentialsModel : Model<SetCredentialsModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
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
        #endregion

        public SetCredentialsModel() { }
        public SetCredentialsModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override bool Write(SetCredentialsModel Model)
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

                AuthenticationService.InitiateSetLocalCredentials(AuthenticationAPI.AuthenticatedId, Model.EmailAddress, Model.Password);

                FeedbackAPI.DisplaySuccess("An email will shortly be sent to " + Model.EmailAddress + " - once confirmed, your credentials will be set");
                return true;
            }
        }
    }
}
