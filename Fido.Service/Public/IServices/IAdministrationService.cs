using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Core;
using Fido.Dtos;
using Fido.Service.Implementation;

namespace Fido.Service
{
    public interface IAdministrationService
    {
        User EnableLocalCredentials(Guid UserId);
        User DisableLocalCredentials(Guid UserId);
        User SetEmailAddress(Guid UserId, string EmailAddress);
        User SetLocalPassword(Guid UserId, string Password);
        User ClearLocalCredentials(Guid UserId);
        User EnableExternalCredentials(Guid UserId);
        User DisableExternalCredentials(Guid UserId);
    }
}
