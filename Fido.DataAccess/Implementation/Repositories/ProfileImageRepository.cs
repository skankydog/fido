using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MoreLinq;
using System.Text;
using RefactorThis.GraphDiff;
using Fido.DataAccess;
using Fido.Entities;
using Fido.Core;

namespace Fido.DataAccess.Implementation
{
    internal class ProfileImageRepository : GenericRepository<ProfileImage>, IProfileImageRepository
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProfileImageRepository(IUnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {}

        public override ProfileImage Insert(ProfileImage Entity)
        {
            throw new NotImplementedException();
        }

        public override ProfileImage Update(ProfileImage Entity)
        {
            Context.UpdateGraph(Entity);

            return Entity;
        }
    }
}
