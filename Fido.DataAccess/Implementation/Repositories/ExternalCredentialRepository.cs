using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Fido.DataAccess;
using Fido.Entities;
using Fido.Core;

namespace Fido.DataAccess.Implementation
{
    internal class ExternalCredentialRepository : GenericRepository<ExternalCredential>, IExternalCredentialRepository
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ExternalCredentialRepository(IUnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {}
    }
}
