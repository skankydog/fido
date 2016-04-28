using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Entities.UserDetails.ExternalCredentialStates
{
    internal class Enabled : IExternalCredentialState
    {
        #region Properties & Constructor
        private User Parent;

        public const string Name_ = "Enabled";
        public string Name { get { return Name_; } }
        public bool ArePresent { get { return true; } }

        public Enabled(User Parent) { this.Parent = Parent; }
        #endregion

        public void Login()
        {
            // Allow
        }

        public void Register(string EmailAddress, string Name)
        {
             // Do nothing - already registered
        }

        //public void Deactivate()
        //{
        //    if (Parent.ExternalCredentials.Count == 0)
        //        Parent.CurrentExternalCredentialState = new None(Parent);
        //}

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
            if (!Parent.CurrentLocalCredentialState.ArePresent && Parent.ExternalCredentials.Count < 2)
                throw new Exception("The account must have local credentials or one or more external credentials");

            // Because the ExternalCredential entity has a composite primary key that includes the
            // id from the user entity, removing it from the User.ExternalCredentials collection
            // and writing back the User will trigger the deletion of the ExternalCredential
            // instance as well...
            var MatchingCredentials =
                from Entities.ExternalCredential e in Parent.ExternalCredentials
                where e.Id == Id
                select e;
            var ExternalCredentialToUnlink = MatchingCredentials.ToList().FirstOrDefault();

            Parent.ExternalCredentials.Remove(ExternalCredentialToUnlink);

            if (Parent.ExternalCredentials.Count == 0)
                Parent.CurrentExternalCredentialState = new None(Parent);
        }

        #region Administration
        public void Enable()
        {
            // Do nothing
        }

        public void Disable()
        {
            Parent.CurrentExternalCredentialState = new Disabled(Parent);
        }
        #endregion
    }
}
