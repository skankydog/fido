using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Core;

namespace Fido.Service.Exceptions
{
    public class PasswordValidationException : ServiceException
    {
        public PasswordValidationException()
            : base("The entered password does not pass validation requirements")
        {
        }
    }
}
