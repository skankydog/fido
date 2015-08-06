using System;

namespace Fido.Dtos
{
    public class ExternalCredential : Dto
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string EmailAddress { get; set; }
    }
}
