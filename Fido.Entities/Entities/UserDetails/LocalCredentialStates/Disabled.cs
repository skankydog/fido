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

        public string StateName { get { return "Disabled"; } }
        public bool HasCredentials { get { return true; } }

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

        public void ExpirePassword()
        {
            throw new Exception("Local credentials are disabled");
        }

        public void Enable()
        {
            Parent.CurrentLocalCredentialState = new Active(Parent);
        }

        public void Disable()
        {
            // Does nothing
        }
    }
}
