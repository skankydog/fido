using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.ExternalCredentialStates
{
    internal class None : IExternalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public const string Name_ = "None";
        public string Name { get { return Name_; } }
        public bool HasCredentials { get { return false; } }

        public None(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            throw new Exception("No external credentials for this account");
        }

        public void Register(string EmailAddress, string Name)
        {
            Parent.Fullname = new Entities.UserDetails.Fullname()
            {
                Firstname = Name.Contains(' ') ? Name.Split(' ').First() : "",
                Surname = Name.Split(' ').Last() // no space returns full string
            };

            Parent.EmailAddress = EmailAddress;
            Parent.CurrentExternalCredentialState = new Enabled(Parent);
        }

        public void Link(string LoginProvider, string ProviderKey, string EmailAddress)
        {
            var ExternalCredentialEntity = new Entities.ExternalCredential()
            {
                UserId = Parent.Id,
                User = Parent,
                LoginProvider = LoginProvider,
                ProviderKey = ProviderKey,
                EmailAddress = EmailAddress
            };

            Parent.ExternalCredentials.Add(ExternalCredentialEntity);
            Parent.CurrentExternalCredentialState = new Enabled(Parent);
        }

        public void Unlink(Guid Id)
        {
            throw new Exception("No external credentials for this account");
        }

        #region Administration
        public void Enable()
        {
            throw new Exception("There are no external credentials to enable");
        }

        public void Disable()
        {
            Parent.CurrentExternalCredentialState = new Disabled(Parent);
        }
        #endregion
    }
}
