using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using RefactorThis.GraphDiff;
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

        //public override ExternalCredential Insert(ExternalCredential Entity)
        //{
        //    Entity.Id = EnsureId(Entity.Id);
        //    Entity.CreatedUtc = EnsureDT(Entity.CreatedUtc);

        //    Log.InfoFormat("Activity.Id='{0}'", Entity.Id);

        //    Context.Set<ExternalCredential>().Add(Entity);

        //    return Entity;
        //}

        public override ExternalCredential Update(ExternalCredential Entity)
        {
            Context.UpdateGraph(Entity);

            return Entity;
        }
    }
}
