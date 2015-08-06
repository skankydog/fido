using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Entities;

namespace Fido.DataAccess
{
    public interface IExternalCredentialRepository : IGenericRepository<ExternalCredential>
    {
    }
}
