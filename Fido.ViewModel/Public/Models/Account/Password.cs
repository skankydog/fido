using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.ViewModel.Implementation;

namespace Fido.ViewModel.Models.Account
{
    public class Password : Model<Password>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "current password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "new password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        #endregion

        public Password()
            : base(ReadAccess: Access.Authenticated, WriteAccess: Access.Authenticated)
        { }

        public override bool Write(Password Model)
        {
            using (new FunctionLogger(Log))
            {
                if (Model.NewPassword != Model.ConfirmPassword)
                {
                    ModelAPI.ModelError("The new password and confirmation password do not match");
                    return false;
                }

                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                if (!AuthenticationService.PasswordPassesValidation(Model.NewPassword))
                {
                    ModelAPI.PropertyError(NewPassword.ToString(), "The password does not meet the minimum validation requirements");
                    return false;
                }

                var UserService = ServiceFactory.CreateService<IUserService>();
                var User = UserService.ChangeLocalPassword(AuthenticationAPI.AuthenticatedId, Model.OldPassword, Model.NewPassword);
                AuthenticationAPI.LoggedInCredentialState = User.LocalCredentialState;

                FeedbackAPI.DisplaySuccess("Your password has been changed");
                return true;
            }
        }
    }
}
