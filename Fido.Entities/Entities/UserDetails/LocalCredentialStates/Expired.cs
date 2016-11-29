using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    public static class Expired
    {
        public const string Name = "Expired";
    }

    internal class ExpiredState : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string Name { get { return Expired.Name; } }
        public bool ArePresent { get { return true; } }
        public bool AreUsable { get { return false; } }

        public ExpiredState(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            // Allow - user will be forced to change their password after login
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
            // Allow
        }

        public void CompleteForgottenPassword(string Password)
        {
            Parent.Password = Password;
            Parent.PasswordLastChangeUtc = DateTime.UtcNow;

            Parent.CurrentLocalCredentialState = new EnabledState(Parent);
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

            Parent.CurrentLocalCredentialState = new EnabledState(Parent);
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

            Parent.CurrentLocalCredentialState = new EnabledState(Parent);
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
