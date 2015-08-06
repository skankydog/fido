using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    internal class Active : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string StateName { get { return "Active"; } }
        public bool HasCredentials { get { return true; } }

        public Active(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            // Do nothing
        }

        public void InitiateRegistration(string EmailAddress, string Password, string Firstname, string Surname)
        {
            throw new Exception("The account already exists - registration can not be performed more than once for an account");
        }

        public void CompleteRegistration()
        {
            throw new Exception("The account already exists - registration can not be performed more than once for an account");
        }

        public void InitiateSetLocalCredentials(string EmailAddress, string Password)
        {
            throw new Exception("The account already exists - registration can not be performed more than once for an account on this account");
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
        }

        public void InitiateChangeEmailAddress()
        {
            // Do nothing
        }

        public void CompleteChangeEmailAddress(string EmailAddress)
        {
            Parent.EmailAddress = EmailAddress;
            Parent.EmailAddressLastChangeUtc = DateTime.UtcNow;
        }

        public void ChangePassword(string Password)
        {
            Parent.Password = Password;
            Parent.PasswordLastChangeUtc = DateTime.UtcNow;
        }

        public void ExpirePassword()
        {
            Parent.CurrentLocalCredentialState = new Expired(Parent);
        }

        public void Enable()
        {
            // Does nothing
        }

        public void Disable()
        {
            Parent.CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Disabled(Parent);
        }
    }
}
