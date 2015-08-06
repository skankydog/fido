using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MoreLinq;
using System.Text;
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

        public User GetByExternalCredentials(string LoginProvider, string ProviderKey)
        {
            using (new FunctionLogger(Log))
            {
                Log.DebugFormat("LoginProvider: {0}", LoginProvider);
                Log.DebugFormat("ProviderKey: {0}", ProviderKey);

                var UserEntity =
                    (from u in Context.Set<User>()
                     join e in Context.Set<ExternalCredential>()
                        on u.Id equals e.UserId
                     where e.LoginProvider == LoginProvider && e.ProviderKey == ProviderKey
                     select u).ToList();

                Log.InfoFormat("Found {0} match(es) for provider/key: {1}/{2}",
                    UserEntity.Count(), LoginProvider, ProviderKey);

                if (UserEntity.Count() == 0)
                    return null;

                if (UserEntity.Count() > 1)
                    throw new ApplicationException(string.Format("More than one match found for provider/key: {0}/{1}",
                        LoginProvider, ProviderKey));

                return UserEntity.First();
            }
        }

        public User GetByExternalEmailAddress(string EmailAddress)
        {
            using (new FunctionLogger(Log))
            {
                Log.DebugFormat("EmailAddress: {0}", EmailAddress);

                var UserEntity =
                    (from u in Context.Set<User>()
                     join e in Context.Set<ExternalCredential>()
                        on u.Id equals e.UserId
                     where e.EmailAddress == EmailAddress
                     select u).DistinctBy(u => u.Id).ToList();

                Log.InfoFormat("Found {0} match(es) for external email address {1}",
                    UserEntity.Count(), EmailAddress);

                if (UserEntity.Count() != 1)
                    return null;

                return UserEntity.First();
            }
        }
    }
}
