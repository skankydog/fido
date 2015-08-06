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

        public string StateName { get { return "None"; } }
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
            Parent.CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.Active(Parent);
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
            Parent.CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.Active(Parent);
        }

        public void Unlink(Guid Id)
        {
            throw new Exception("No external credentials for this account");
        }

        public void Enable()
        {
            throw new Exception("The external credentials are not currently disabled");
        }

        public void Disable()
        {
            Parent.CurrentExternalCredentialState = new UserDetails.ExternalCredentialStates.Disabled(Parent);
        }
    }
}
