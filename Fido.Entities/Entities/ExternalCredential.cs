using System;

namespace Fido.Entities
{
    public class ExternalCredential : Entity
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string EmailAddress { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
