using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Core;

namespace Fido.Service.Exceptions
{
    public class ServiceException : ExceptionBase
    {
        public ServiceException(string Message)
            : base(Message)
        {
        }
    }
}
