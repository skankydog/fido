using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.ExternalCredentialStates
{
    internal class Active : IExternalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public string StateName { get { return "Active"; } }
        public bool HasCredentials { get { return true; } }

        public Active(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            // Do nothing
        }

        public void Register(string EmailAddress, string Name)
        {
             // Do nothing - already registered
        }

        public void Deactivate()
        {
            if (Parent.ExternalCredentials.Count == 0)
                Parent.CurrentExternalCredentialState = new None(Parent);
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
        }

        public void Unlink(Guid Id)
        {
            if (!Parent.CurrentLocalCredentialState.HasCredentials && Parent.ExternalCredentials.Count < 2)
                throw new Exception("The account must have local credentials or one or more external credentials");

            // Because the ExternalCredential entity has a composite primary key that includes the
            // id from the user entity, removing it from the User.ExternalCredentials collection
            // and writing back the User will trigger the deletion of the ExternalCredential
            // instance as well...
            var MatchingCredentials =
                from Entities.ExternalCredential E in Parent.ExternalCredentials
                where E.Id == Id
                select E;
            var ExternalCredentialToUnlink = MatchingCredentials.ToList().FirstOrDefault();

            Parent.ExternalCredentials.Remove(ExternalCredentialToUnlink);

            if (Parent.ExternalCredentials.Count == 0)
                Parent.CurrentExternalCredentialState = new None(Parent);
        }

        public void Enable()
        {
            throw new Exception("The external credentials are not currently disabled");
        }

        public void Disable()
        {
            Parent.CurrentExternalCredentialState = new Disabled(Parent);
        }
    }
}
