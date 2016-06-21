using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models.Account
{
    public class SetLocalCredentialComplete : Model<SetLocalCredentialComplete>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid ConfirmationId { get; set; }
        #endregion

    //    public SetLocalCredentialComplete() { }
        public SetLocalCredentialComplete()
            //IFeedbackAPI FeedbackAPI,
            //IAuthenticationAPI LoginAPI,
            //IModelAPI ModelAPI)
                : base (//FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override bool Save(SetLocalCredentialComplete Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
                var User = AuthenticationService.SetLocalCredentialComplete(Model.ConfirmationId);

                AuthenticationAPI.SignOut();
                AuthenticationAPI.SignIn(User.Id, User.Fullname.FirstnameSurname, false);
                AuthenticationAPI.LoggedInCredentialState = User.LocalCredentialState;
                FeedbackAPI.DisplaySuccess("Your local credentials have been confirmed.");

                return true;
            }
        }
    }
}
