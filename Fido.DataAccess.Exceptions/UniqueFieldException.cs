using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Core;
using Fido.Core.Exceptions;

namespace Fido.DataAccess.Exceptions
{
    public class UniqueFieldException : ExceptionBase
    {
        public UniqueFieldException(string Message, Exception InnerException)
            : base(Message, InnerException)
        {
        }
    }
}
