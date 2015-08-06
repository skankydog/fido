using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ResetPasswordModel : Model<ResetPasswordModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public Guid ConfirmationId { get; set; }
        #endregion

        public ResetPasswordModel() { } // pure model
        public ResetPasswordModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: false)
        { }

        public override bool Write(ResetPasswordModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                if (!AuthenticationService.PasswordPassesValidation(Model.Password))
                {
                    ModelAPI.ModelError("The new password does not meet minimum policy requirements.");
                    return false;
                }

                var User = AuthenticationService.CompleteForgottenPassword(Model.ConfirmationId, Model.Password);
                AuthenticationAPI.SignIn(User.Id, User.Fullname.DisplayName, false);
                AuthenticationAPI.LoggedInCredentialState = User.LocalCredentialState;

                FeedbackAPI.DisplaySuccess("Your password has been changed - welcome back " + User.Fullname.Firstname + ".");
                return true;
            }
        }
    }
}
