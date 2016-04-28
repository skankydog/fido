using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    internal class Enabled : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public const string Name_ = "Enabled";
        public string Name { get { return Name_; } }
        public bool ArePresent { get { return true; } }
        public bool AreUsable { get { return true; } }

        public Enabled(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            // Allow
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
        }

        public void InitiateChangeEmailAddress()
        {
            // Allow
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

        #region Administration
        public void Expire()
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

            Parent.CurrentLocalCredentialState = new None(Parent);
        }
        #endregion
    }
}
