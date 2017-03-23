using System;
using System.Collections.Generic;
using Fido.Core;
using Fido.Service;
using Fido.ViewModel.Implementation;

namespace Fido.ViewModel.Models.Account
{
    public class ExternalCredential : Model<ExternalCredential>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string EmailAddress { get; set; }
        #endregion

        public ExternalCredential()
            : base(ReadAccess: Access.NA, WriteAccess: Access.NA)
        { }

        public override bool Write(ExternalCredential Model)
        {
            using (new FunctionLogger(Log))
            {
                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

                if (Model.Id == null || Model.Id == Guid.Empty)
                {
                    AuthenticationService.LinkExternalCredentials(
                        AuthenticationAPI.AuthenticatedId,
                        Model.LoginProvider,
                        Model.ProviderKey,
                        Model.EmailAddress);

                    FeedbackAPI.DisplaySuccess("The external credentials have been linked to your account.");
                }
                else
                {
                    AuthenticationService.UnlinkExternalCredentials(
                        AuthenticationAPI.AuthenticatedId,
                        Model.Id);

                    FeedbackAPI.DisplaySuccess("The external credentials have been removed");
                }

                return true;
            }
        }
    }
}
