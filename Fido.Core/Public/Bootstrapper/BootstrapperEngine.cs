using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fido.Core.Bootstrapper;

namespace Fido.Core.Bootstrapper
{
    public static class BootstrapperEngine
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Bootstrap()
        {
            using (new FunctionLogger(Log))
            {
                TypeFinder Finder = new TypeFinder(Assembly.GetCallingAssembly());

                foreach (Type Bootstrapper in Finder.Find<IBootstrapper>())
                {
                    Log.InfoFormat("Bootstrapping: {0}", Bootstrapper.FullName);
                    IBootstrapper Instance = (IBootstrapper)Activator.CreateInstance(Bootstrapper);
                    Instance.Initialise();
                }
            }
        }
    }
}
