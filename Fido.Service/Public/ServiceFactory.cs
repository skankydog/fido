using System;
using System.Collections.Generic;
using Fido.Core.Bootstrapper;

namespace Fido.Service
{
    public static class ServiceFactory
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Boot()
        {
            BootstrapperEngine.Bootstrap();
        }

        public static ISERVICE CreateService<ISERVICE>()
        {
            var Assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var FullPath = string.Concat(Assembly.GetName().Name, ".Implementation.", typeof(ISERVICE).Name.Substring(1));

            System.Type ServiceType = Assembly.GetType(FullPath);
            return (ISERVICE)Activator.CreateInstance(ServiceType);
        }
    }
}
