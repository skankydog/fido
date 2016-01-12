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
        public ProfileImage ProfileImage { get; set; }
        public string About { get; set; }

        public ICollection<Role> Roles { get; set; }

        #region Local Credential Properties
        public bool HasLocalCredentials { get { return CurrentLocalCredentialState.HasCredentials; } }
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public ILocalCredentialState CurrentLocalCredentialState { get; internal set; }

        public string LocalCredentialState
        {
            get { return CurrentLocalCredentialState.Name; }
            set
            {
                switch (value)
                {
                    case UserDetails.LocalCredentialStates.None.Name_:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.None(this);
                        break;

                    case UserDetails.LocalCredentialStates.Registered.Name_:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Registered(this);
                        break;

                    case UserDetails.LocalCredentialStates.Expired.Name_:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Expired(this);
                        break;

                    case UserDetails.LocalCredentialStates.Enabled.Name_:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Enabled(this);
                        break;

                    case UserDetails.LocalCredentialStates.Disabled.Name_:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Disabled(this);
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Local state type {0} not implemented", value));
                }
            }
        }

        public DateTime? EmailAddressLastChangeUtc { get; set; } // NULL if no local credentials
        public int? EmailAddressAgeDays { get { return EmailAddressLastChangeUtc == null ? (int?)null : Convert.ToInt16((DateTime.UtcNow - (DateTime)EmailAddressLastChangeUtc).TotalDays); } }
        public DateTime? PasswordLastChangeUtc { get; set; } // NULL if no local credentials
        #endregion

        #region External Credential Properties
        public bool HasExternalCredentials { get { return CurrentExternalCredentialState.HasCredentials; } }
        public IList<ExternalCredential> ExternalCredentials { get; set; }
        public IExternalCredentialState CurrentExternalCredentialState { get; internal set; }

        public string ExternalCredentialState
        {
            get { return CurrentExternalCredentialState.Name; }
            set
            {
                switch (value)
                {
                    case UserDetails.ExternalCredentialStates.None.Name_:
                    //case "None":
                        CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.None(this);
                        break;

                    case UserDetails.ExternalCredentialStates.Enabled.Name_:
                    //case "Enabled":
                        CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.Enabled(this);
                        break;

                    case UserDetails.ExternalCredentialStates.Disabled.Name_:
                    //case "Disabled":
                        CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.Disabled(this);
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Exteral state type {0} not implemented", value));
                }
            }
        }
        #endregion

        #region Administration
        public void SetLocalCredentialState(string State)
        {
            switch (State)
            {
                case UserDetails.LocalCredentialStates.Expired.Name_:
                    CurrentLocalCredentialState.Expire();
                    break;

                case UserDetails.LocalCredentialStates.Enabled.Name_:
                    CurrentLocalCredentialState.Enable();
                    break;

                case UserDetails.LocalCredentialStates.Disabled.Name_:
                    CurrentLocalCredentialState.Disable();
                    break;

                case UserDetails.LocalCredentialStates.None.Name_:
                case null:
                    break; // Do nothing - can't set to None

                default:
                    throw new Exception(string.Format("Local credential state, {0}, not valid", State));
            }
        }

        public void SetExternalCredentialState(string State)
        {
            switch (State)
            {
                case UserDetails.ExternalCredentialStates.Enabled.Name_:
                    CurrentExternalCredentialState.Enable();
                    break;

                case UserDetails.ExternalCredentialStates.Disabled.Name_:
                    CurrentExternalCredentialState.Disable();
                    break;

                case UserDetails.ExternalCredentialStates.None.Name_:
                case null:
                    break; // Do nothing - can't set to None

                default:
                    throw new Exception(string.Format("External credential state, {0}, not valid", ExternalCredentialState));
            }
        }
        #endregion

        public User()
        {
            CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.None(this);
            CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.None(this);

            Fullname = new Fullname();
            ExternalCredentials = new List<ExternalCredential>();
            Roles = new List<Role>();
        }
    }
}
