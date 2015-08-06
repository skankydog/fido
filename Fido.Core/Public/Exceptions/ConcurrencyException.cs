using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.Core.Exceptions
{
    public class ConcurrencyException : ExceptionBase
    {
        public ConcurrencyException(string Message, Exception InnerException)
            : base(Message, InnerException)
        { }

        public ConcurrencyException(string Message)
            : base (Message)
        { }
    }
}
