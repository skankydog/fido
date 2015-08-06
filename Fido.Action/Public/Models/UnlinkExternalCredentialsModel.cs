using System;
using System.Collections.Generic;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class UnlinkExternalCredentialsModel : Model<UnlinkExternalCredentialsModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        #endregion

        public UnlinkExternalCredentialsModel() { } // pure model
        public UnlinkExternalCredentialsModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override bool Write(UnlinkExternalCredentialsModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                AuthenticationService.UnlinkExternalCredentials(
                    AuthenticationAPI.AuthenticatedId,
                    Model.Id);

                FeedbackAPI.DisplaySuccess("the external credentials have been removed");
                return true;
            }
        }
    }
}
