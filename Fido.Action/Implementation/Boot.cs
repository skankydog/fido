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
                BootPermissions();
            }
        }

        private void BootPermissions()
        {
            using (new FunctionLogger(Log))
            {
                var ModelTypes = new TypeFinder().Find<ILogicModel>().Where(t => t.Namespace.StartsWith("Fido.Action.Models."));

                foreach (var ModelType in ModelTypes)
                {
                    var ModelInstance = (ILogicModel)Activator.CreateInstance(ModelType);
                    var Area = string.Join(string.Empty, ModelType.Namespace.Skip("Fido.Action.Models.".Length)); // to do: remove magic string

                    if (ModelInstance.RequiresReadPermission)
                        Ensure(ModelType.Name, Area, Permission.Read.ToString());

                    if (ModelInstance.RequiresWritePermission)
                        Ensure(ModelType.Name, Area, Permission.Write.ToString());
                }

                var RoleService = ServiceFactory.CreateService<IRoleService>();
                RoleService.SetAdministrationRole("Fido Administrator");
            }
        }

        private void Ensure(string Name, string Area, string Action)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Name: {0}", Name);
                Log.InfoFormat("Area: {0}", Area);
                Log.InfoFormat("Action: {0}", Action);

                var ActivityService = ServiceFactory.CreateService<IActivityService>();

                if (ActivityService.Get(Name, Area, Action) == null)
                {
                    Log.InfoFormat("Adding activity: {0}, {1}, {2}", Name, Area, Action);
                    ActivityService.Save(new Dtos.Activity { Name = Name, Area = Area, Action = Action });
                }
            }
        }
    }
}
