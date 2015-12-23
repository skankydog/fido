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
    internal class ConfigurationRepository : GenericRepository<Configuration>, IConfigurationRepository
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConfigurationRepository(IUnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {}

        public override Configuration Insert(Configuration Entity)
        {
            throw new NotImplementedException();
        }

        public override Configuration Update(Configuration Entity)
        {
            Context.UpdateGraph(Entity);

            return Entity;
        }
    }
}
