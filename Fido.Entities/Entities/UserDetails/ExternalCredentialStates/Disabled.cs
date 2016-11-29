using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.ExternalCredentialStates
{
    public static class Disabled
    {
        public const string Name = "Disabled";
    }

    internal class DisabledState : IExternalCredentialState
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
            throw new Exception("External login disabled");
        }

        public void Register(string EmailAddress, string Name)
        {
            throw new Exception("External login disabled");
        }

        public void Link(string LoginProvider, string ProviderKey, string EmailAddress)
        {
            throw new Exception("External login disabled");
        }

        public void Unlink(Guid Id)
        {
            throw new Exception("External login disabled");
        }

        #region Administration
        public void Enable()
        {
            Parent.CurrentExternalCredentialState = new EnabledState(Parent);
        }

        public void Disable()
        {
            // Do nothing
        }
        #endregion
    }
}
