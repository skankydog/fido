using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class LocalLoginModel : Model<LocalLoginModel>
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

        public LocalLoginModel() { }
        public LocalLoginModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: false)
        { }

        public override bool Write(LocalLoginModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
                var User = AuthenticationService.LoginByLocalCredentials(Model.EmailAddress, Model.Password);

                if (User != null)
                {
                    AuthenticationAPI.SignIn(User.Id, User.Fullname.DisplayName, Model.RememberMe);
                    AuthenticationAPI.LoggedInCredentialState = User.LocalCredentialState;

                    return true;
                }

                ModelAPI.ModelError("Invalid username, password combination.");
                return false;
            }
        }
    }
}
