using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Dtos;

namespace Fido.Service
{
    public interface IConfigurationService
    {
        Configuration Get();
        void Set(Configuration Configuration);
    }
}
