﻿using System;
using System.Collections.Generic;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ExternalCredential : Model<ExternalCredential>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid Id { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string EmailAddress { get; set; }
        #endregion

        public ExternalCredential() { }
        public ExternalCredential(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: false, RequiresWritePermission: false)
        { }

        public override bool Save(ExternalCredential Model)
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