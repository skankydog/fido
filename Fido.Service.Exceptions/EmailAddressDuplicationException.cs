using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Core;

namespace Fido.Service.Exceptions
{
    public class EmailAddressDuplicationException : ServiceException
    {
        public EmailAddressDuplicationException(string EmailAddress)
            : base(string.Format("The email address, {0}, is already in use.", EmailAddress))
        {
        }
    }
}
