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
        public int EmailAddressAgeDays { get; set; }
        public DateTime? PasswordLastChangeUtc { get; set; }
        public int PasswordAgeDays { get; set; }
        #endregion

        #region External Credentials
        public bool HasExternalCredentials { get; set; }
        public string ExternalCredentialState { get; set; }
        public IList<ExternalCredential> ExternalCredentials { get; set; }

        public bool HasLoginProvider(string LoginProvider)
        {
            var Matches = from e in ExternalCredentials
                          where e.LoginProvider.ToLower() == LoginProvider.ToLower()
                          select e;

            return Matches.Count() > 0;
        }
        #endregion

        public IList<Role> Roles { get; set; }
    }
}
