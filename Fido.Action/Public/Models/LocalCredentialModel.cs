using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class LocalCredentialModel : Model<LocalCredentialModel>, IModelCRUD
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public enum _Action
        {
            ExpirePassword = 0,
            Disable,
            Enable,
            SetEmailAddress,
            SetPassword,
            Clear,
        }

        public Guid Id { get; set; }

        public _Action Action { get; set; }

        public string EmailAddress { get; set; }
        public string Password { get; set;}
        public string LocalCredentialState { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedUtc { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }

     //   public string InputState { get; set; }
        #endregion

        public LocalCredentialModel() { } // pure model
        public LocalCredentialModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override LocalCredentialModel Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var AdministrationService = ServiceFactory.CreateService<IAdministrationService>();

                // TO DO

                return new LocalCredentialModel();
            }
        }

        public override bool Write(LocalCredentialModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var AdministrationService = ServiceFactory.CreateService<IAdministrationService>();

                // TO DO

                return true;
            }
        }
    }
}
