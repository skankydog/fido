using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails
{
    public interface IExternalCredentialState
    {
        string Name { get; }
        bool ArePresent { get; }
        bool AreUsable { get; }


        void Login();
        void Register(string EmailAddress, string Name);

        void Link(string LoginProvider, string ProviderKey, string EmailAddress);
        void Unlink(Guid Id);

        #region Administration
        void Enable();
        void Disable();
        #endregion
    }
}
