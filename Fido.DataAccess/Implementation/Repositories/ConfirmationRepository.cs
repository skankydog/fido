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
    internal class ConfirmationRepository : GenericRepository<Confirmation>, IConfirmationRepository
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConfirmationRepository(IUnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {}

        //public override Confirmation Insert(Confirmation Entity)
        //{
        //    Entity.Id = EnsureId(Entity.Id);
        //    Entity.CreatedUtc = EnsureDT(Entity.CreatedUtc);

        //    Log.InfoFormat("Activity.Id='{0}'", Entity.Id);

        //    Context.Set<Confirmation>().Add(Entity);

        //    return Entity;
        //}

        public override Confirmation Update(Confirmation Entity)
        {
            Context.UpdateGraph(Entity);

            return Entity;
        }

        public override void Delete(Guid Id)
        {
            var Entity = Get(Id);

            Delete(Entity);
        }

        public override void Delete(Confirmation Entity)
        {
            if (!Entity.Deletable)
                throw new Exception("The confirmation is not in a state that allows it to be deleted");

            base.Delete(Entity);
        }
    }
}
