using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models.Authentication
{
    public class Login : Model<Login>
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

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        #endregion

        public Login()
            : base(ReadAccess: Access.NA, WriteAccess: Access.NA)
        { }

        public override bool Write(Login Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
                var User = AuthenticationService.LoginByLocalCredentials(Model.EmailAddress, Model.Password);

                if (User != null)
                {
                    AuthenticationAPI.SignIn(User.Id, User.Fullname.FirstnameSurname, Model.RememberMe);
                    AuthenticationAPI.LoggedInCredentialState = User.LocalCredentialState;

                    return true;
                }

                FeedbackAPI.DisplayError("Invalid username, password combination.");
                ModelAPI.ModelError("Invalid username, password combination.");
                return false;
            }
        }
    }
}
