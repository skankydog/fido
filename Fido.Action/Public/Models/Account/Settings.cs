using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class Settings : Model<Settings>
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
        #endregion

        public Settings() { }
        public Settings(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true, RequiresWritePermission: true)
        { }

        public override Settings Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var SettingsDto = UserService.GetSettings(Id);

                var Model = AutoMapper.Mapper.Map<Dtos.Settings, Settings>(SettingsDto);

                return Model;
            }
        }
    }
}
