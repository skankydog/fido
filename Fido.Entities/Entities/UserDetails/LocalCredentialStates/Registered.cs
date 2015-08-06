using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    internal class Registered : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string StateName { get { return "Registered"; } }
        public bool HasCredentials { get { return true; } }

        public Registered(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            throw new Exception("The registration for this account has not yet been confirmed");
        }

        public void InitiateRegistration(string EmailAddress, string Password, string Firstname, string Surname)
        {
            // Do nothing
        }

        public void CompleteRegistration()
        {
            Parent.CurrentLocalCredentialState = new Active(Parent);
        }

        public void InitiateSetLocalCredentials(string EmailAddress, string Password)
        {
            throw new Exception("Local credentials have already been registered");
        }

        public void CompleteSetLocalCredentials()
        {
            Parent.CurrentLocalCredentialState = new Active(Parent);
        }

        public void InitiateForgottenPassword()
        {
            throw new Exception("The registration for this account has not yet been confirmed");
        }

        public void CompleteForgottenPassword(string Password)
        {
            throw new Exception("The registration for this account has not yet been confirmed");
        }

        public void CompleteChangeEmailAddress()
        {
            throw new Exception("The registration for this account has not yet been confirmed");
        }

        public void InitiateChangeEmailAddress()
        {
            throw new Exception("The registration for this account has not yet been confirmed");
        }

        public void CompleteChangeEmailAddress(string EmailAddress)
        {
            Parent.EmailAddress = EmailAddress;
            Parent.EmailAddressLastChangeUtc = DateTime.UtcNow;

            Parent.CurrentLocalCredentialState = new Active(Parent);
        }

        public void ChangePassword(string Password)
        {
            throw new Exception("Passords cannot be changed as the registration has not yet been confirmed");
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
            Parent.CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Disabled(Parent);
        }
    }
}
