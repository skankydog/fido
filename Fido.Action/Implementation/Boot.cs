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
                var ModelTypes = new TypeFinder().Find<ILogicModel>();

                foreach (var ModelType in ModelTypes)
                {
                    var Model = (ILogicModel)Activator.CreateInstance(ModelType);

            //        if (Model.ReadAccess == Access.Permissioned)
                        AddPermission(Model, Function.Read);
                        //Ensure(Model.ModelArea, Model.ModelName, Function.Read);

            //        if (Model.WriteAccess == Access.Permissioned)
                        AddPermission(Model, Function.Write);
                        //Ensure(Model.ModelArea, Model.ModelName, Function.Write);
                }

                var RoleService = ServiceFactory.CreateService<IRoleService>();
                RoleService.SetAdministrationRole("Administrator");
            }
        }

        private void AddPermission(ILogicModel Model, Function Function)
        {
            using (new FunctionLogger(Log))
            {
                var RequiredAccess = Function == Function.Write ? Model.WriteAccess : Model.ReadAccess;

                if (RequiredAccess != Access.Permissioned)
                {
                    Log.InfoFormat("Model {0} does not require permission for {1}", Model.ModelName, Function.ToString());
                    return;
                }

                var Funct = Function.ToString();

                Log.InfoFormat("Area: {0}", Model.ModelArea);
                Log.InfoFormat("Name: {0}", Model.ModelName);
                Log.InfoFormat("Function: {0}", Funct);

                var ActivityService = ServiceFactory.CreateService<IActivityService>();

                if (ActivityService.Get(Funct, Model.ModelName, Model.ModelArea) == null)
                {
                    Log.InfoFormat("Adding activity: {0}.{1}.{2}", Model.ModelArea, Model.ModelName, Funct);
                    ActivityService.Save(new Dtos.Activity { Area = Model.ModelArea, Name = Model.ModelName, Action = Funct });
                }
            }
        }

        //private void Ensure(string Area, string Name, Function Function)
        //{
        //    using (new FunctionLogger(Log))
        //    {
        //        var Funct = Function.ToString();

        //        Log.InfoFormat("Area: {0}", Area);
        //        Log.InfoFormat("Name: {0}", Name);
        //        Log.InfoFormat("Function: {0}", Funct);

        //        var ActivityService = ServiceFactory.CreateService<IActivityService>();

        //        if (ActivityService.Get(Funct, Name, Area) == null)
        //        {
        //            Log.InfoFormat("Adding activity: {0}.{1}.{2}", Area, Name, Funct);
        //            ActivityService.Save(new Dtos.Activity { Area = Area, Name = Name, Action = Funct });
        //        }
        //    }
        //}
    }
}
