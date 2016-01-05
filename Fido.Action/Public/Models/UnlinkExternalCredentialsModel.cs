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
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

        public UnlinkExternalCredentialsModel()
        {
            Id = Guid.NewGuid();
            CreatedUtc = DateTime.UtcNow;
            IsNew = true;
        }

        public UnlinkExternalCredentialsModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
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
