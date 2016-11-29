using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    public static class Registered
    {
        public const string Name = "Registered";
    }

    internal class RegisteredState : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string Name { get { return Registered.Name; } }
        public bool ArePresent { get { return true; } }
        public bool AreUsable { get { return false; } }

        public RegisteredState(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            throw new Exception("The registration for this account has not yet been confirmed");
        }

        public void InitiateRegistration(string EmailAddress, string Password, string Firstname, string Surname)
        {
            // Allow
        }

        public void CompleteRegistration()
        {
            Parent.CurrentLocalCredentialState = new EnabledState(Parent);
        }

        public void InitiateSetLocalCredentials(string EmailAddress, string Password)
        {
            throw new Exception("Local credentials have already been registered");
        }

        public void CompleteSetLocalCredentials()
        {
            Parent.CurrentLocalCredentialState = new EnabledState(Parent);
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

            Parent.CurrentLocalCredentialState = new EnabledState(Parent);
        }

        public void ChangePassword(string Password)
        {
            throw new Exception("Passords cannot be changed as the registration has not yet been confirmed");
        }

        #region Administration
        public void Expire()
        {
            // Does nothing
        }

        public void Enable()
        {
            Parent.CurrentLocalCredentialState = new EnabledState(Parent);
        }

        public void Disable()
        {
            Parent.CurrentLocalCredentialState = new DisabledState(Parent);
        }

        public void SetEmailAddress(string EmailAddress)
        {
            Parent.EmailAddressLastChangeUtc = DateTime.UtcNow;
            Parent.EmailAddress = EmailAddress;
        }

        public void SetPassword(string Password)
        {
            Parent.PasswordLastChangeUtc = DateTime.UtcNow;
            Parent.Password = Password;
        }

        public void Clear()
        {
            Parent.EmailAddressLastChangeUtc = DateTime.UtcNow;
            Parent.PasswordLastChangeUtc = DateTime.UtcNow;

            Parent.EmailAddress = null;
            Parent.Password = null;

            Parent.CurrentLocalCredentialState = new NoneState(Parent);
        }
        #endregion
    }
}
