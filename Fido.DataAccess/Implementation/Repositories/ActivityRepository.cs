using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using RefactorThis.GraphDiff; // new
using Fido.DataAccess;
using Fido.Entities;
using Fido.Core;

namespace Fido.DataAccess.Implementation
{
    internal class ActivityRepository : GenericRepository<Activity>, IActivityRepository
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActivityRepository(IUnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {}

        public override Activity Insert(Activity Entity)
        {
            Log.InfoFormat("Activity.Id='{0}'", Entity.Id);

            Context.Set<Activity>().Add(Entity);
            return Entity;
        }

        public override Activity Update(Activity Entity)
        {
            Context.UpdateGraph(Entity);

            return Entity;
        }
    }
}
