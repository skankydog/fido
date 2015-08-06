using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fido.Core.Bootstrapper;

namespace Fido.DataAccess
{
    public static class DataAccessFactory
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Boot()
        {
            BootstrapperEngine.Bootstrap();
        }

        public static IUnitOfWork CreateUnitOfWork()
        {
            var Assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var FullPath = string.Concat(Assembly.GetName().Name, ".Implementation.UnitOfWork");
            System.Type UnitOfWorkType = Assembly.GetType(FullPath);

            return (IUnitOfWork)Activator.CreateInstance(UnitOfWorkType);
        }

        public static IREPOSITORY CreateRepository<IREPOSITORY>(IUnitOfWork UnitOfWork)
        {
            var Assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var FullPath = string.Concat(Assembly.GetName().Name, ".Implementation.", typeof(IREPOSITORY).Name.Substring(1));
            System.Type RepositoryType = Assembly.GetType(FullPath);

            return (IREPOSITORY)Activator.CreateInstance(RepositoryType, UnitOfWork);
        }

        public static IDataPrimer CreateDataPrimer()
        {
            var Assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var FullPath = string.Concat(Assembly.GetName().Name, ".Implementation.DataPrimer");
            System.Type UnitOfWorkType = Assembly.GetType(FullPath);

            return (IDataPrimer)Activator.CreateInstance(UnitOfWorkType);
        }
    }
}
