using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.ExternalCredentialStates
{
    internal class Disabled : IExternalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public const string Name_ = "Disabled";
        public string Name { get { return Name_; } }
        public bool HasCredentials { get { return true; } }

        public Disabled(User Parent) { this.Parent = Parent; }
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
            Parent.CurrentExternalCredentialState = new Enabled(Parent);
        }

        public void Disable()
        {
            // Do nothing
        }
        #endregion
    }
}
