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
        // Local Credentials...
        public bool HasLocalCredentials { get; set; } // Read only
        [Display(Name = "email address")]
        public string EmailAddress { get; set; } // Read only
        [Display(Name = "local credential state")]
        public string LocalCredentialState { get; set; } // Read only   
        public int DaysUntilPasswordExpires { get; set; } // Read only
        public bool PasswordChangePolicy { get; set; } // Read only

        // External Credentials...
        public bool HasExternalCredentials { get; set; } // Read only
        [Display(Name = "external credential state")]
        public string ExternalCredentialState { get; set; } // Read only
        public IList<ExternalCredential> ExternalCredentials { get; set; } // Read only

        public class ExternalCredential
        {
            public Guid Id { get; set; }
            public string LoginProvider { get; set; }
            public string EmailAddress { get; set; }
        }
        #endregion

        public SettingsModel() { } // pure model
        public SettingsModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresAuthentication: true)
        { }

        public override SettingsModel Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var ConfigurationService = ServiceFactory.CreateService<IConfigurationService>();

                var User = UserService.Get(Id, "ExternalCredentials");
                var Configuration = ConfigurationService.Get();

                return MapToSettings(User, Configuration);
            }
        }

        private SettingsModel MapToSettings(Dtos.User User, Dtos.Configuration Configuration)
        {
            var Model = new SettingsModel()
                {
                    HasLocalCredentials = User.HasLocalCredentials,
                    LocalCredentialState = User.LocalCredentialState,
                    EmailAddress = User.EmailAddress,
                    PasswordChangePolicy = Configuration.PasswordChangePolicy,
                    DaysUntilPasswordExpires = Configuration.PasswordChangePolicyDays - User.PasswordAgeDays,
                    
                    HasExternalCredentials = User.HasExternalCredentials,
                    ExternalCredentialState = User.ExternalCredentialState,
                    ExternalCredentials = new List<ExternalCredential>()
                };

            foreach (var Credential in User.ExternalCredentials)
            {
                Model.ExternalCredentials.Add(new ExternalCredential { Id = Credential.Id, LoginProvider = Credential.LoginProvider, EmailAddress = Credential.EmailAddress });
            }

            return Model;
        }
    }
}
