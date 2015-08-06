using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Core;

namespace Fido.Service.Exceptions
{
    public class EmailAddressValidationException : ServiceException
    {
        public EmailAddressValidationException()
            : base("The entered email address does not pass validation requirements or is already being used by another user")
        {
        }

        public EmailAddressValidationException(string EmailAddress)
            : base("The email address, " + EmailAddress + ", does not pass validation requirements")
        {
        }
    }
}
