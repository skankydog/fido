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
    internal class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RoleRepository(IUnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {}

        protected override string DefaultIncludes { get { return "Activities, Users"; } }

        public override Role Insert(Role Entity)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Role.Id='{0}'", Entity.Id);

                Entity = Context.Set<Role>().Add(Entity);

                foreach (var User in Entity.Users)
                    Context.Entry(User).State = System.Data.Entity.EntityState.Unchanged;

                foreach (var Activity in Entity.Activities)
                    Context.Entry(Activity).State = System.Data.Entity.EntityState.Unchanged;

                return Entity;
            }
        }

        public override Role Update(Role Entity)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Role.Id='{0}'", Entity.Id);

                Context.UpdateGraph(Entity, Map => Map.AssociatedCollection(Role => Role.Activities));
                Context.UpdateGraph(Entity, Map => Map.AssociatedCollection(Role => Role.Users));

                return Entity;
            }
        }
    }
}
