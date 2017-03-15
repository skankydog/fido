// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models.Administration
{
    public class Configuration : Model<Configuration>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public int PasswordChangePolicyDays { get; set; }
        public bool PasswordChangePolicy { get; set; }
        #endregion

        public Configuration()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
        { }

        public override Configuration Prepare(Configuration Model)
        {
            //var RoleService = ServiceFactory.CreateService<IRoleService>();
            //Model.AllRoles = Mapper.Map<IList<Dtos.Role>, IList<Role>>(RoleService.GetAll().OrderBy(r => r.Name).ToList());

            return Model;
        }

        public override Configuration Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var ConfigurationService = ServiceFactory.CreateService<IConfigurationService>();

                var Configuration = ConfigurationService.Get();
                var Model = Mapper.Map<Dtos.Configuration, Configuration>(Configuration);

                return Model;
            }
        }

        public override bool Write(Configuration Model)
        {
            using (new FunctionLogger(Log))
            {
                var ConfigurationDto = Mapper.Map<Configuration, Dtos.Configuration>(Model);

                var ConfigurationService = ServiceFactory.CreateService<IConfigurationService>();
                ConfigurationService.Set(ConfigurationDto);

                FeedbackAPI.DisplaySuccess("The configuration has been saved");
                return true;
            }
        }
    }
}
