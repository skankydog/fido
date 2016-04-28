using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    internal class Disabled : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public const string Name_ = "Disabled";
        public string Name { get { return Name_; } }
        public bool ArePresent { get { return true; } }
        public bool AreUsable { get { return false; } }

        public Disabled(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            throw new Exception("Local credentials are disabled");
        }

        public void InitiateRegistration(string EmailAddress, string Password, string Firstnme, string Surname)
        {
            throw new Exception("Local credentials are disabled");
        }

        public void CompleteRegistration()
        {
            throw new Exception("Local credentials are disabled");
        }

        public void InitiateSetLocalCredentials(string EmailAddress, string Password)
        {
            throw new Exception("The account already exists - registration can not be performed more than once for an account");
        }

        public void CompleteSetLocalCredentials()
        {
            throw new Exception("The account already exists - registration can not be performed more than once for an account");
        }

        public void InitiateForgottenPassword()
        {
            throw new Exception("Local credentials are disabled");
        }

        public void CompleteForgottenPassword(string Password)
        {
            throw new Exception("Local credentials are disabled");
        }

        public void InitiateChangeEmailAddress()
        {
            throw new Exception("Local credentials are disabled");
        }

        public void CompleteChangeEmailAddress(string EmailAddress)
        {
            throw new Exception("Local credentials are disabled");
        }

        public void ChangePassword(string Password)
        {
            throw new Exception("Local credentials are disabled");
        }

        #region Administration
        public void Expire()
        {
            throw new Exception("Local credentials are disabled");
        }

        public void Enable()
        {
            if (string.IsNullOrEmpty(Parent.EmailAddress) ||
                string.IsNullOrEmpty(Parent.Password))
            {
                throw new Exception("Unable to enable credentials - missing email address and/or password");
            }

            Parent.CurrentLocalCredentialState = new Enabled(Parent);
        }

        public void Disable()
        {
            // Does nothing
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
