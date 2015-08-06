using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails
{
    public interface ILocalCredentialState
    {
        string StateName { get; }
        bool HasCredentials { get; }

        void Login();

        void InitiateRegistration(string EmailAddress, string Password, string Firstname, string Surname);
        void CompleteRegistration();

        void InitiateSetLocalCredentials(string EmailAddress, string Password);
        void CompleteSetLocalCredentials();

        void InitiateForgottenPassword();
        void CompleteForgottenPassword(string Password);
        
        void InitiateChangeEmailAddress();
        void CompleteChangeEmailAddress(string EmailAddress);

        void ChangePassword(string Password);
        void ExpirePassword();

        void Enable();
        void Disable();
    }
}
