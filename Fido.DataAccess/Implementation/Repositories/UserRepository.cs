using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MoreLinq;
using RefactorThis.GraphDiff; // new
using Fido.DataAccess;
using Fido.Entities;
using Fido.Core;

namespace Fido.DataAccess.Implementation
{
    internal class UserRepository : GenericRepository<User>, IUserRepository
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public UserRepository(IUnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {}

        protected override string DefaultIncludes { get { return "Roles, ExternalCredentials"; } }

        public override User Update(User Entity)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("User.Id='{0}'", Entity.Id);

                Context.UpdateGraph(Entity, Map => Map.AssociatedCollection(User => User.Roles));
                Context.UpdateGraph(Entity, Map => Map.OwnedCollection(User => User.ExternalCredentials));

                return Entity;
            }
        }

        public User GetByExternalCredentials(string LoginProvider, string ProviderKey, string IncludeProperties = null)
        {
            using (new FunctionLogger(Log))
            {
                IncludeProperties = IncludeProperties.IsNullOrEmpty() ? DefaultIncludes : IncludeProperties;
                Log.DebugFormat("LoginProvider: {0}", LoginProvider);
                Log.DebugFormat("ProviderKey: {0}", ProviderKey);
                Log.DebugFormat("IncludeProperties: {0}", IncludeProperties);

                //var UserEntity =
                //    (from u in Context.Set<User>()
                //     join e in Context.Set<ExternalCredential>()
                //        on u.Id equals e.UserId
                //     where e.LoginProvider == LoginProvider && e.ProviderKey == ProviderKey
                //     select u).AsNoTracking().ToList();

                IQueryable<User> Query =
                    (from u in Context.Set<User>()
                     join e in Context.Set<ExternalCredential>()
                        on u.Id equals e.UserId
                     where e.LoginProvider == LoginProvider && e.ProviderKey == ProviderKey
                     select u).AsNoTracking();

                if (IncludeProperties != null)
                {
                    foreach (var IncludeProperty in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        Query = Query.Include(IncludeProperty);
                }

                var DistinctList = Query.DistinctBy(u => u.Id).ToList();
                return DistinctList.Count() == 0 ? null : DistinctList.First();

                //Log.InfoFormat("Found {0} match(es) for provider/key: {1}/{2}",
                //    UserEntity.Count(), LoginProvider, ProviderKey);

                //if (UserEntity.Count() == 0)
                //    return null;

                //if (UserEntity.Count() > 1)
                //    throw new ApplicationException(string.Format("More than one match found for provider/key: {0}/{1}",
                //        LoginProvider, ProviderKey));

                //return UserEntity.First();
            }
        }

        public User GetByExternalEmailAddress(string EmailAddress, string IncludeProperties = null)
        {
            using (new FunctionLogger(Log))
            {
                IncludeProperties = IncludeProperties.IsNullOrEmpty() ? DefaultIncludes : IncludeProperties;
                Log.DebugFormat("EmailAddress: {0}", EmailAddress);
                Log.DebugFormat("IncludeProperties: {0}", IncludeProperties);

                //var UserEntity =
                //    (from u in Context.Set<User>()
                //     join e in Context.Set<ExternalCredential>()
                //        on u.Id equals e.UserId
                //     where e.EmailAddress == EmailAddress
                //     select u).AsNoTracking().DistinctBy(u => u.Id).ToList();

                IQueryable<User> Query =
                    (from u in Context.Set<User>()
                     join e in Context.Set<ExternalCredential>()
                        on u.Id equals e.UserId
                     where e.EmailAddress == EmailAddress
                     select u).AsNoTracking();

                if (IncludeProperties != null)
                {
                    foreach (var IncludeProperty in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        Query = Query.Include(IncludeProperty);
                }

                var DistinctList = Query.DistinctBy(u => u.Id).ToList();
                return DistinctList.Count() == 0 ? null : DistinctList.First();
            }
        }
    }
}
