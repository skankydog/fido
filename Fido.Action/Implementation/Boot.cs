using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fido.Core;
using Fido.Core.Bootstrapper;

namespace Fido.Service.Implementation
{
    class Boot : IBootstrapper
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Initialise()
        {
            using (new FunctionLogger(Log))
            {
                Service.ServiceFactory.Boot();
            }
        }
    }
}
