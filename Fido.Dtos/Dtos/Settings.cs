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

        public bool HasLocalCredentials { get; set; }
        public string LocalCredentialState { get; set; }

        public bool HasExternalCredentials { get; set; }
        public string ExternalCredentialState { get; set; }
        public IList<ExternalCredential> ExternalCredentials { get; set; }

        public bool PasswordChangePolicy { get; set; }
        public int PasswordChangePolicyDays { get; set; }

        public int PasswordAgeDays { get; set; }
    }
}
