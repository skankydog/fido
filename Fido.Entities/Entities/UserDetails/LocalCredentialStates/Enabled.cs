using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    public static class Enabled
    {
        public const string Name = "Enabled";
    }

    internal class EnabledState : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string Name { get { return Enabled.Name; } }
        public bool ArePresent { get { return true; } }
        public bool AreUsable { get { return true; } }

        public EnabledState(User Parent) { this.Parent = Parent; }
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

        public void Expire()
        {
            Parent.CurrentLocalCredentialState = new ExpiredState(Parent);
        }
    }
}
