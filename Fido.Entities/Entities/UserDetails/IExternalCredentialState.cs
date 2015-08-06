using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails
{
    public interface IExternalCredentialState
    {
        string StateName { get; }
        bool HasCredentials { get; }

        void Login();
        void Register(string EmailAddress, string Name);

        void Link(string LoginProvider, string ProviderKey, string EmailAddress);
        void Unlink(Guid Id);
        
        void Enable();
        void Disable();
    }
}
