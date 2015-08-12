using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Dtos
{
    public class User : Dto
    {
        public string EmailAddress { get; set; }
        public Fullname Fullname { get; set; }
        public string About { get; set; }

        #region Local Credentials
        public bool HasLocalCredentials { get; set; }
        public string LocalCredentialState { get; set; }
        public DateTime? EmailAddressLastChangeUtc { get; set; }
        public DateTime? PasswordLastChangeUtc { get; set; }
        #endregion

        #region External Credentials
        public bool HasExternalCredentials { get; set; }
        public string ExternalCredentialState { get; set; }
        public IList<ExternalCredential> ExternalCredentials { get; set; }
        #endregion
    }
}
