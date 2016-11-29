using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    public static class None
    {
        public const string Name = "None";
    }

    internal class NoneState : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string Name { get { return None.Name; } }
        public bool ArePresent { get { return false; } }
        public bool AreUsable { get { return false; } }

        public NoneState(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            throw new Exception("No local credentials");
        }

        public void InitiateRegistration(string EmailAddress, string Password, string Firstname, string Surname)
        {
            Parent.EmailAddress = EmailAddress;
            Parent.Password = Password;
            Parent.Fullname.Firstname = Firstname;
            Parent.Fullname.Surname = Surname;

            DateTime Now = DateTime.UtcNow;
            Parent.EmailAddressLastChangeUtc = Now;
            Parent.PasswordLastChangeUtc = Now;

            Parent.CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.RegisteredState(Parent);
        }

        public void CompleteRegistration()
        {
            throw new Exception("No local credentials");
        }

        public void InitiateSetLocalCredentials(string EmailAddress, string Password)
        {
            Parent.EmailAddress = EmailAddress;
            Parent.Password = Password;

            DateTime Now = DateTime.UtcNow;
            Parent.EmailAddressLastChangeUtc = Now;
            Parent.PasswordLastChangeUtc = Now;

            Parent.CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.RegisteredState(Parent);
        }

        public void CompleteSetLocalCredentials()
        {
            throw new Exception("The account has not be registered for local credentials");
        }

        public void InitiateForgottenPassword()
        {
            throw new Exception("No local credentials");
        }

        public void CompleteForgottenPassword(string Password)
        {
            throw new Exception("No local credentials");
        }

        public void InitiateChangeEmailAddress()
        {
            throw new Exception("No local credentials");
        }

        public void CompleteChangeEmailAddress(string EmailAddress)
        {
            throw new Exception("No local credentials");
        }

        public void ChangePassword(string Password)
        {
            throw new Exception("No local credentials");
        }

        #region Administration
        public void Expire()
        {
            throw new Exception("No local credentials");
        }

        public void Enable()
        {
            throw new Exception("No local credentials");
        }

        public void Disable()
        {
            Parent.CurrentLocalCredentialState = new DisabledState(Parent);
        }

        public void SetEmailAddress(string EmailAddress)
        {
            Parent.EmailAddressLastChangeUtc = DateTime.UtcNow;
            Parent.EmailAddress = EmailAddress;

            if (Parent.Password == null)
                return; // doesn't change state

            Parent.CurrentLocalCredentialState = new EnabledState(Parent);
        }

        public void SetPassword(string Password)
        {
            Parent.PasswordLastChangeUtc = DateTime.UtcNow;
            Parent.Password = Password;

            if (Parent.EmailAddress == null)
                return; // doesn't change state

            Parent.CurrentLocalCredentialState = new EnabledState(Parent);
        }

        public void Clear()
        {
            // Does nothing
        }
        #endregion
    }
}
