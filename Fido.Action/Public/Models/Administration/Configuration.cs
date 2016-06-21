using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.Action.Models.Administration
{
    public class Configuration : Model<Configuration>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public IList<Role> AllRoles = new List<Role>();

        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<Guid> SelectedRoles { get; set; }

        [Display(Name = "created date")]
        public DateTime CreatedUtc { get; set; }
        [Display(Name = "record age")]
        public int? CreatedAgeDays { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

   //     public Configuration() { }
        public Configuration()
            //IFeedbackAPI FeedbackAPI,
            //IAuthenticationAPI LoginAPI,
            //IModelAPI ModelAPI)
                : base (//FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true,
                        RequiresWritePermission: true)
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

        public override bool Save(Configuration Model)
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
