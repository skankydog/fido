using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class User : Dto
    {
        public string EmailAddress { get; set; } // Read only
        public Fullname Fullname { get; set; }
        public string About { get; set; }

        #region Local Credentials
        public bool HasLocalCredentials { get; set; } // Read only
        public string LocalCredentialState { get; set; } // Read only
        public DateTime? EmailAddressLastChangeUtc { get; set; } // Read only
        public DateTime? PasswordLastChangeUtc { get; set; } // Read only

        public int PasswordAgeDays { get { return PasswordLastChangeUtc != null ? 0 : (DateTime.UtcNow - CreatedUtc).Days; } }
        #endregion

        #region External Credentials
        public string ExternalCredentialState { get; set; } // Read only
        public bool HasExternalCredentials { get; set; } // Read only
        public IList<ExternalCredential> ExternalCredentials { get; set; } // Read only
        #endregion
    }
}
