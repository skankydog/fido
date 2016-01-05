using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class SettingsModel : Model<SettingsModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public bool HasLocalCredentials { get; set; }

        [Display(Name = "email address")]
        public string EmailAddress { get; set; }
        
        [Display(Name = "local credential state")]
        public string LocalCredentialState { get; set; }   
        
        public int DaysUntilPasswordExpires { get; set; }
        public bool PasswordChangePolicy { get; set; }

        public bool HasExternalCredentials { get; set; }
        
        [Display(Name = "external credential state")]
        public string ExternalCredentialState { get; set; }
        
        public IList<ExternalCredential> ExternalCredentials { get; set; }

        public class ExternalCredential
        {
            public Guid Id { get; set; }
            public string LoginProvider { get; set; }
            public string EmailAddress { get; set; }
        }
        #endregion

        public SettingsModel() { }
        public SettingsModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override SettingsModel Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var SettingsDto = UserService.GetSettings(Id);

                HasLocalCredentials = SettingsDto.HasLocalCredentials;
                LocalCredentialState = SettingsDto.LocalCredentialState;
                EmailAddress = SettingsDto.EmailAddress;
                PasswordChangePolicy = SettingsDto.PasswordChangePolicy;
                DaysUntilPasswordExpires = SettingsDto.PasswordChangePolicyDays - SettingsDto.PasswordAgeDays;
                HasExternalCredentials = SettingsDto.HasExternalCredentials;
                ExternalCredentialState = SettingsDto.ExternalCredentialState;
                ExternalCredentials = new List<ExternalCredential>();

                foreach (var ExternalCredential in SettingsDto.ExternalCredentials)
                    ExternalCredentials.Add(new ExternalCredential 
                        { 
                            Id = ExternalCredential.Id, 
                            LoginProvider = ExternalCredential.LoginProvider, 
                            EmailAddress = ExternalCredential.EmailAddress 
                        });

                return this;
            }
        }
    }
}
