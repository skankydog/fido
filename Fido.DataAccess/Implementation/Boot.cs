using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using Fido.Core.Bootstrapper;
using Fido.Core;

namespace Fido.DataAccess.Implementation
{
    class Boot : IBootstrapper
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Initialise()
        {
            using (new FunctionLogger(Log))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DataAccess-Mode"] == "Development")
                {
                    Log.Debug("Development mode enabled");
                    Database.SetInitializer<Context>(new DropCreateDatabaseAlways<Context>());

                    using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                    {
                        var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                        UserRepository.SetUnique("EmailAddress");

                        var RoleRepository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
                        RoleRepository.SetUnique("Name");

                        var ActivityRepository = DataAccessFactory.CreateRepository<IActivityRepository>(UnitOfWork);
                        ActivityRepository.SetUnique("Name");

                        ////var ExternalLoginRepository = DataAccessFactory.CreateRepository<IExternalLoginRepository>(UnitOfWork);
                        ////ExternalLoginRepository.Index("LoginProvider");
                        ////ExternalLoginRepository.Index("ProviderKey"); // Should be a composite index?
                    }
                }
            }
        }
    }
}
