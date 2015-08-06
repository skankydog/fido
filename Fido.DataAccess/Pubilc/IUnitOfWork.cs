using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fido.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
