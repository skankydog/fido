using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class Settings
    {
        public string EmailAddress { get; set; }
        public Fullname Fullname { get; set; }

        public bool LocalCredentialsArePresent { get; set; }
        public bool LocalCredentialsAreUsable { get; set; }
        public string LocalCredentialState { get; set; }

        public bool ExternalCredentialsArePresent { get; set; }
        public string ExternalCredentialState { get; set; }
        public IList<ExternalCredential> ExternalCredentials { get; set; }

        public bool PasswordChangePolicy { get; set; }
        public int PasswordChangePolicyDays { get; set; }

        public int PasswordAgeDays { get; set; }
        public int DaysUntilPasswordExpires { get { return PasswordChangePolicy == false ? 0 : PasswordChangePolicyDays - PasswordAgeDays; } }
    }
}
