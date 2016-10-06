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
            const string ROOT_NAMESPACE = "Fido.Action.Models.";

            using (new FunctionLogger(Log))
            {
                var ModelTypes = new TypeFinder().Find<ILogicModel>().Where(t => t.Namespace.StartsWith(ROOT_NAMESPACE));

                foreach (var ModelType in ModelTypes)
                {
                    var ModelInstance = (ILogicModel)Activator.CreateInstance(ModelType);
                    var Area = string.Join(string.Empty, ModelType.Namespace.Skip(ROOT_NAMESPACE.Length));

                    if (ModelInstance.ReadAccess == Access.Permissioned)
                        Ensure(ModelType.Name, Area, Function.Read);

                    if (ModelInstance.WriteAccess == Access.Permissioned)
                        Ensure(ModelType.Name, Area, Function.Write);
                }

                var RoleService = ServiceFactory.CreateService<IRoleService>();
                RoleService.SetAdministrationRole("Administrator");
            }
        }

        private void Ensure(string Name, string Area, Function Action)
        {
            using (new FunctionLogger(Log))
            {
                var ActionName = Action.ToString();

                Log.InfoFormat("Name: {0}", Name);
                Log.InfoFormat("Area: {0}", Area);
                Log.InfoFormat("Action: {0}", ActionName);

                var ActivityService = ServiceFactory.CreateService<IActivityService>();

                if (ActivityService.Get(Name, Area, ActionName) == null)
                {
                    Log.InfoFormat("Adding activity: {0}, {1}, {2}", Name, Area, ActionName);
                    ActivityService.Save(new Dtos.Activity { Name = Name, Area = Area, Action = ActionName });
                }
            }
        }
    }
}
