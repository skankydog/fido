using System;
using System.Collections.Generic;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class LinkExternalCredentialsCallbackModel : Model<LinkExternalCredentialsCallbackModel
        >
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string EmailAddress { get; set; }
        #endregion

        public LinkExternalCredentialsCallbackModel() { } // pure model
        public LinkExternalCredentialsCallbackModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override bool Write(LinkExternalCredentialsCallbackModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                AuthenticationService.LinkExternalCredentials(
                        AuthenticationAPI.AuthenticatedId,
                        Model.LoginProvider,
                        Model.ProviderKey,
                        Model.EmailAddress);

                FeedbackAPI.DisplaySuccess("The external credentials have been linked to your account.");
                return true;
            }
        }
    }
}
