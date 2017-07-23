using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.ViewModel.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.ViewModel.Models.Administration
{
    public class Role : Model<Role>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public IList<Activity> AllActivities = new List<Activity>();

        [Display(Name = "role name")]
        [Required(ErrorMessage = "The role name field cannot be left blank")]
        public string Name { get; set; }

        public IList<Guid> SelectedActivities { get; set; }
        #endregion

        public Role()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
        { }

        public override Role Prepare(Role Model)
        {
            var ActivityService = ServiceFactory.CreateService<IActivityService>();
            Model.AllActivities = Mapper.Map<IList<Dtos.Activity>, IList<Activity>>(ActivityService.GetAll().OrderBy(a => a.Name).ToList());

            return Model;
        }

        public override Role Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var RoleService = ServiceFactory.CreateService<IRoleService>();

                var Role = RoleService.Get(Id);
                var Model = Mapper.Map<Dtos.Role, Role>(Role);

                return Model;
            }
        }

        public override bool Write(Role Model)
        {
            using (new FunctionLogger(Log))
            {
                var RoleDto = Mapper.Map<Role, Dtos.Role>(Model);

                RoleDto.Activities = Model.SelectedActivities == null ? new List<Dtos.Activity>()
                : Mapper.Map<IList<Activity>, IList<Dtos.Activity>>(
                    (from a in Model.AllActivities
                     where (Model.SelectedActivities.Contains(a.Id))
                     select a).ToList());

                var RoleService = ServiceFactory.CreateService<IRoleService>();
                RoleDto = RoleService.Save(RoleDto);

                FeedbackAPI.DisplaySuccess("The role details have been saved");
                return true;
            }
        }

        public override bool Delete(Role Model)
        {
            using (new FunctionLogger(Log))
            {
                var RoleService = ServiceFactory.CreateService<IRoleService>();

                RoleService.Delete(Model.Id);

                FeedbackAPI.DisplaySuccess("The role record has been deleted");
                return true;
            }
        }
    }
}
