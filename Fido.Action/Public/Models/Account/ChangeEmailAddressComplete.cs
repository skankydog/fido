using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models.Account
{
    public class ChangeEmailAddressComplete : Model<ChangeEmailAddressComplete>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid ConfirmationId { get; set; }
        #endregion

        public ChangeEmailAddressComplete()
            : base(RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override bool Save(ChangeEmailAddressComplete Model)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();

                UserService.ChangeEmailAddressComplete(Model.ConfirmationId);
                FeedbackAPI.DisplaySuccess("The email address for this account has been successfully changed.");

                return true;
            }
        }
    }
}
