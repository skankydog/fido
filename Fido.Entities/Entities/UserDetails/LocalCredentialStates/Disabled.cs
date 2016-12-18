using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.LocalCredentialStates
{
    public static class Disabled
    {
        public const string Name = "Disabled";
    }

    internal class DisabledState : ILocalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string Name { get { return Disabled.Name; } }
        public bool ArePresent { get { return true; } }
        public bool AreUsable { get { return false; } }

        public DisabledState(User Parent) { this.Parent = Parent; }
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

        public void Expire()
        {
            throw new Exception("Local credentials are disabled");
        }
    }
}
