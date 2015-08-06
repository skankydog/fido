using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    internal class Expired : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string StateName { get { return "Expired"; } }
        public bool HasCredentials { get { return true; } }

        public Expired(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            // Does nothing - user will be forced to change their password after login
        }

        public void InitiateRegistration(string EmailAddress, string Password, string Firstname, string Surname)
        {
            throw new Exception("The account already exists");
        }

        public void CompleteRegistration()
        {
            throw new Exception("The local password is in an expired state");
        }

        public void InitiateSetLocalCredentials(string EmailAddress, string Password)
        {
            throw new Exception("Local credentials already set");
        }

        public void CompleteSetLocalCredentials()
        {
            throw new Exception("Local credentials already set");
        }

        public void InitiateForgottenPassword()
        {
            // Do nothing
        }

        public void CompleteForgottenPassword(string Password)
        {
            Parent.Password = Password;
            Parent.PasswordLastChangeUtc = DateTime.UtcNow;

            Parent.CurrentLocalCredentialState = new Active(Parent);
        }

        public void InitiateChangeEmailAddress()
        {
            throw new Exception("The local password is in an expired state");
        }

        public void CompleteChangeEmailAddress(string EmailAddress)
        {
            throw new Exception("The local password is in an expired state");
        }

        public void ChangePassword(string Password)
        {
            Parent.Password = Password;
            Parent.PasswordLastChangeUtc = DateTime.UtcNow;

            Parent.CurrentLocalCredentialState = new Active(Parent);
        }

        public void ExpirePassword()
        {
            // Does nothing
        }

        public void Enable()
        {
            throw new Exception("The local credentials are not currently disabled");
        }

        public void Disable()
        {
            Parent.CurrentLocalCredentialState = new Disabled(Parent);
        }
    }
}
