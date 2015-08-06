using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Entities.UserDetails;

namespace Fido.Entities
{
    public class User : Entity
    {
        public Fullname Fullname { get; set; }
        public IList<Role> Roles { get; set; }
        public ProfileImage ProfileImage { get; set; }
        public string About { get; set; }

        #region Local Credentials
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public ILocalCredentialState CurrentLocalCredentialState { get; internal set; }

        public string LocalCredentialState
        {
            get { return CurrentLocalCredentialState.StateName; }
            set
            {
                switch (value)
                {
                    case "None":
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.None(this);
                        break;

                    case "Registered":
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Registered(this);
                        break;

                    case "Expired":
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Expired(this);
                        break;

                    case "Active":
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Active(this);
                        break;

                    case "Disabled":
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Disabled(this);
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Local state type {0} not implemented", value));
                }
            }
        }

        public DateTime? EmailAddressLastChangeUtc { get; set; } // NULL if no local credentials
        public DateTime? PasswordLastChangeUtc { get; set; } // NULL if no local credentials
        #endregion

        #region External Credentials
        public IList<ExternalCredential> ExternalCredentials { get; set; }
        public IExternalCredentialState CurrentExternalCredentialState { get; internal set; }

        public string ExternalCredentialState
        {
            get { return CurrentExternalCredentialState.StateName; }
            set
            {
                switch (value)
                {
                    case "None":
                        CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.None(this);
                        break;

                    case "Active":
                        CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.Active(this);
                        break;

                    case "Disabled":
                        CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.Disabled(this);
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Exteral state type {0} not implemented", value));
                }
            }
        }
        #endregion

        public User()
        {
            CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.None(this);
            CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.None(this);

            Fullname = new Fullname();
            Roles = new List<Role>();
            ExternalCredentials = new List<ExternalCredential>();
        }
    }
}
