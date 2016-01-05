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
                var ModelTypes = new TypeFinder().Find<IModel>().Where(t => t.Namespace == "Fido.Action.Models");

                foreach (var ModelType in ModelTypes)
                {
                    var ModelInstance = (IModel)Activator.CreateInstance(ModelType, null, null, null);

                    if (ModelInstance.RequiresReadPermission)
                        EnsurePermission(ModelInstance.GetType().Name, Permission.Read);

                    if (ModelInstance.RequiresWritePermission)
                        EnsurePermission(ModelInstance.GetType().Name, Permission.Write);
                }

                var RoleService = ServiceFactory.CreateService<IRoleService>();
                RoleService.SetAdministrationRole("Fido Administrator");
            }
        }

        private void EnsurePermission(string Name, Permission Permission)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Name: {0}", Name);
                Log.InfoFormat("Permission: {0}", Permission.ToString());

                // TO DO: Also look here for inconsistencies and throw if found. Basically, if there is a model set
                //        to RequiresWritePermissions that does not overload Write or Delete, or one with
                //        RequiresReadPermission and does not overload read, we want to throw an exception as this
                //        is not valid - you can't check for the opposite, of course, as having a write that does
                //        not require permission is actually valid.
                var ActivityService = ServiceFactory.CreateService<IActivityService>();
                string ActivityName = string.Format("{0}-{1}", Name, Permission.ToString());

                if (ActivityService.GetByName(ActivityName) == null)
                {
                    Log.InfoFormat("Adding activity: {0}", ActivityName);
                    ActivityService.Save(new Dtos.Activity { Name = ActivityName });
                }
            }
        }
    }
}
