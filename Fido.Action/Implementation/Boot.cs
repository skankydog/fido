using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fido.Core;
using Fido.Core.Bootstrapper;
using Fido.Service;

namespace Fido.Action.Implementation
{
    class Boot : IBootstrapper
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Initialise()
        {
            using (new FunctionLogger(Log))
            {
                ServiceFactory.Boot();

                BootAuthorisation();
            }
        }

        private void BootAuthorisation()
        {
            using (new FunctionLogger(Log))
            {
                var ActivityService = ServiceFactory.CreateService<IActivityService>();
                var ModelTypes = new TypeFinder().Find<IModel>().Where(t => t.Namespace == "Fido.Action.Models");

                foreach (var ModelType in ModelTypes)
                {
                    var ModelInstance = (IModel)Activator.CreateInstance(ModelType, null, null, null);

                    if (ModelInstance.RequiresAuthentication == true)
                    {
                        string Name = ModelInstance.GetType().Name;

                        if (ActivityService.GetByName(Name) == null)
                        {
                            Log.InfoFormat("Adding activity: {0}", Name);
                            ActivityService.Save(new Dtos.Activity { Name = Name });
                        }
                    }
                }

                var RoleService = ServiceFactory.CreateService<IRoleService>();
                RoleService.SetAdministrationRole("Fido Administrator");
                //RoleService.SetAdministrationRole("Role02");
            }
        }
    }
}
