using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    internal class None : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string StateName { get { return "None"; } }
        public bool HasCredentials { get { return false; } }

        public None(User Parent) { this.Parent = Parent; }
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

            Parent.CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Registered(Parent);
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

            Parent.CurrentLocalCredentialState = new UserDetails.LocalCredentialStates.Registered(Parent);
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

        public void ExpirePassword()
        {
            throw new Exception("No local email address");
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
