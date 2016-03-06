//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Linq.Expressions;
//using MoreLinq;
//using System.Text;
//using RefactorThis.GraphDiff;
//using Fido.DataAccess;
//using Fido.Entities;
//using Fido.Core;

//namespace Fido.DataAccess.Implementation
//{
//    internal class UserImageRepository : GenericRepository<UserImage>, IUserImageRepository
//    {
//        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

//        public UserImageRepository(IUnitOfWork UnitOfWork)
//            : base(UnitOfWork)
//        { }

//        public override UserImage Insert(UserImage Entity)
//        {
//            throw new NotImplementedException();
//        }

//        public override UserImage Update(UserImage Entity)
//        {
//            Context.UpdateGraph(Entity);

//            return Entity;
//        }
//    }
//}
