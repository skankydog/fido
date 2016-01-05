using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class LocalCredentialModel : Model<LocalCredentialModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public enum _Action // Don't believe I need this!
        {
            ExpirePassword = 0,
            Disable,
            Enable,
            SetEmailAddress,
            SetPassword,
            Clear,
        }

        public Guid Id { get; set; }

        public _Action Action { get; set; } // Don't believe I need this!

        public string EmailAddress { get; set; }
        public string Password { get; set;}
        public string LocalCredentialState { get; set; } // Don't believe I need this!

        // Don't believe I need any of this...
        [Display(Name = "Created Date")]
        public DateTime CreatedUtc { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

        public LocalCredentialModel()
        {
            Id = Guid.NewGuid();
            CreatedUtc = DateTime.UtcNow;
            IsNew = true;
        }

        public LocalCredentialModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override LocalCredentialModel Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                // TO DO

                return new LocalCredentialModel();
            }
        }

        public override bool Write(LocalCredentialModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                // TO DO

                FeedbackAPI.DisplaySuccess("The account has been successfully updated");
                return true;
            }
        }

        public override bool Delete(LocalCredentialModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                // TO DO

                FeedbackAPI.DisplaySuccess("The local credentials have been deleted");
                return true;
            }
        }
    }
}
