using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class RegistrationComplete : Model<RegistrationComplete>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid ConfirmationId { get; set; }
        #endregion

        public RegistrationComplete() { }
        public RegistrationComplete(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: false, RequiresWritePermission: false)
        { }

        public override bool Save(RegistrationComplete Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
                var User = AuthenticationService.RegistrationComplete(Model.ConfirmationId);

                AuthenticationAPI.SignOut();
                AuthenticationAPI.SignIn(User.Id, User.Fullname.FirstnameSurname, false);
                AuthenticationAPI.LoggedInCredentialState = User.LocalCredentialState;
                FeedbackAPI.DisplaySuccess("Your local credentials have been confirmed.");

                return true;
            }
        }
    }
}
