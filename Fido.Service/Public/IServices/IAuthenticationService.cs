using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Core;
using Fido.Dtos;
using Fido.Service.Implementation;

namespace Fido.Service
{
    public interface IAuthenticationService
    {
        bool HasLocalCredentials(Guid UserId);
        bool HasExternalCredentials(Guid UserId);

        User LoginByLocalCredentials(string EmailAddress, string Password);
        User LoginByExternalCredentials(string LoginProvider, string ProviderKey);
        User LoginByExternalEmailAddress(string LoginProvider, string ProviderKey, string EmailAddress);
        User CreateByExternalCredentials(string LoginProvider, string ProviderKey, string EmailAddress, string Name);

        Guid RegistrationInitiate(string EmailAddress, string Password, string Firstname, string Surname);
        User RegistrationComplete(Guid ConfirmationId);
        Guid SetLocalCredentialInitiate(Guid UserId, string EmailAddress, string Password);
        User SetLocalCredentialComplete(Guid ConfirmationId);
        Guid ForgottenPasswordInitiate(string EmailAddress);
        Confirmation ForgottenPasswordReceive(Guid ConfirmationId);
        User ForgottenPasswordComplete(Guid ConfirmationId, string NewPassword);
        
        bool PasswordPassesValidation(string Password);
        PasswordScore GetPasswordScore(string Password);

        bool EmailAddressPassesValidation(string EmailAddress);
        bool EmailAddressIsFree(string EmailAddress);           // Email addresses must be unique
        
        IList<ExternalCredential> GetExternalCredentials(Guid UserId);
        void LinkExternalCredentials(Guid UserId, string LoginProvider, string ProviderKey, string EmailAddress);
        void UnlinkExternalCredentials(Guid UserId, Guid Id);
    }
}
