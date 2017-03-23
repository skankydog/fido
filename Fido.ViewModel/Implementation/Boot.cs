using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fido.Core;
using Fido.Core.Bootstrapper;
using Fido.Service;

namespace Fido.ViewModel.Implementation
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
                var ModelTypes = new TypeFinder().Find<ILogicModel>();

                foreach (var ModelType in ModelTypes)
                {
                    var Model = (ILogicModel)Activator.CreateInstance(ModelType);

                    AddPermission(Model, ReadWrite.Read);
                    AddPermission(Model, ReadWrite.Write);
                }

                var RoleService = ServiceFactory.CreateService<IRoleService>();
                RoleService.SetAdministrationRole("Administrator");
            }
        }

        private void AddPermission(ILogicModel Model, ReadWrite ReadWrite)
        {
            using (new FunctionLogger(Log))
            {
                var RequiredAccess = ReadWrite == ReadWrite.Write ? Model.WriteAccess : Model.ReadAccess;

                if (RequiredAccess != Access.Permissioned)
                {
                    Log.InfoFormat("Model {0} does not require permission for {1}", Model.ModelName, ReadWrite.ToString());
                    return;
                }

                var RW = ReadWrite.ToString();

                Log.InfoFormat("Area: {0}", Model.ModelArea);
                Log.InfoFormat("Name: {0}", Model.ModelName);
                Log.InfoFormat("Function: {0}", RW);

                var ActivityService = ServiceFactory.CreateService<IActivityService>();

                if (ActivityService.Get(Model.ModelArea, Model.ModelName, RW) == null)
                {
                    Log.InfoFormat("Adding activity: {0}.{1}.{2}", Model.ModelArea, Model.ModelName, RW);
                    ActivityService.Save(new Dtos.Activity { Area = Model.ModelArea, Name = Model.ModelName, ReadWrite = RW });
                }
            }
        }
    }
}
