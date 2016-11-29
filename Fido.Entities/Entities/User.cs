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
        public string About { get; set; }
        public UserImage UserImage { get; set; } // Do I just store the image in the end???
        public ICollection<Role> Roles { get; set; }

        #region Local Credential Properties
        public bool LocalCredentialsArePresent { get { return CurrentLocalCredentialState.ArePresent; } }
        public bool LocalCredentialsAreUsable { get { return CurrentLocalCredentialState.AreUsable; } }
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
                    case UserDetails.LocalCredentialStates.None.Name:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.NoneState(this);
                        break;

                    case UserDetails.LocalCredentialStates.Registered.Name:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.RegisteredState(this);
                        break;

                    case UserDetails.LocalCredentialStates.Expired.Name:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.ExpiredState(this);
                        break;

                    case UserDetails.LocalCredentialStates.Enabled.Name:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.EnabledState(this);
                        break;

                 //   case UserDetails.LocalCredentialStates.DisabledState.Name_:
                    case UserDetails.LocalCredentialStates.Disabled.Name:
                        CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.DisabledState(this);
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Local credential state type {0} not implemented", value));
                }
            }
        }

        public DateTime? EmailAddressLastChangeUtc { get; set; } // NULL if no local credentials
        public int? EmailAddressAgeDays { get { return EmailAddressLastChangeUtc == null ? (int?)null : Convert.ToInt16((DateTime.UtcNow - (DateTime)EmailAddressLastChangeUtc).TotalDays); } }
        public DateTime? PasswordLastChangeUtc { get; set; } // NULL if no local credentials
        public int? PasswordAgeDays { get { return PasswordLastChangeUtc == null ? (int?)null : Convert.ToInt16((DateTime.UtcNow - (DateTime)PasswordLastChangeUtc).TotalDays); } }
        #endregion

        #region External Credential Properties
        public bool ExternalCredentialsArePresent { get { return CurrentExternalCredentialState.ArePresent; } }
        public bool ExternalCredentialsAreUsable { get { return CurrentExternalCredentialState.AreUsable; } }
        public IList<ExternalCredential> ExternalCredentials { get; set; }
        public IExternalCredentialState CurrentExternalCredentialState { get; internal set; }

        public string ExternalCredentialState
        {
            get { return CurrentExternalCredentialState.Name; }
            set
            {
                switch (value)
                {
                    case UserDetails.ExternalCredentialStates.None.Name:
                        CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.NoneState(this);
                        break;

                    case UserDetails.ExternalCredentialStates.Enabled.Name:
                        CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.EnabledState(this);
                        break;

                    case UserDetails.ExternalCredentialStates.Disabled.Name:
                        CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.DisabledState(this);
                        break;

                    default:
                        throw new NotImplementedException(string.Format("External credential state type {0} not implemented", value));
                }
            }
        }
        #endregion

        //#region Administration
        //public void SetLocalCredentialState(string State)
        //{
        //    switch (State)
        //    {
        //        case UserDetails.LocalCredentialStates.Expired.Name_:
        //            CurrentLocalCredentialState.Expire();
        //            break;

        //        case UserDetails.LocalCredentialStates.Enabled.Name_:
        //            CurrentLocalCredentialState.Enable();
        //            break;

        //        case UserDetails.LocalCredentialStates.Disabled.Name_:
        //            CurrentLocalCredentialState.Disable();
        //            break;

        //        case UserDetails.LocalCredentialStates.None.Name_:
        //            Password = null;
        //            EmailAddress = null;
        //            LocalCredentialState = "None";
        //            break;

        //        default:
        //            throw new Exception(string.Format("Local credential state, '{0}', not valid", State));
        //    }
        //}

        //public void SetExternalCredentialState(string State)
        //{
        //    switch (State)
        //    {
        //        case UserDetails.ExternalCredentialStates.Enabled.Name_:
        //            CurrentExternalCredentialState.Enable();
        //            break;

        //        case UserDetails.ExternalCredentialStates.Disabled.Name_:
        //            CurrentExternalCredentialState.Disable();
        //            break;

        //        case UserDetails.ExternalCredentialStates.None.Name_:
        //            // TO DO:How do I delete all the external credentials here????
        //            ExternalCredentials.Clear(); // I wonder if this works!!??
        //            ExternalCredentialState = State;
        //            break;

        //        default:
        //            throw new Exception(string.Format("External credential state, {0}, not valid", ExternalCredentialState));
        //    }
        //}
        //#endregion

        public User()
        {
            CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.NoneState(this);
            CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.NoneState(this);

            Fullname = new Fullname();
            ExternalCredentials = new List<ExternalCredential>();
            Roles = new List<Role>();
        }
    }
}
