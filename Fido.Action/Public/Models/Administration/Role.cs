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
    public class Role : Model<Role>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public IList<Activity> AllActivities = new List<Activity>();
    //    public IList<User> AllUsers = new List<User>();

        public Guid Id { get; set; }

        [Display(Name = "role name")]
        [Required(ErrorMessage = "The role name field cannot be left blank")]
        public string Name { get; set; }

        public IList<Guid> SelectedActivities { get; set; }
    //    public IList<Guid> SelectedUsers { get; set; }

        [Display(Name = "created date")]
        public DateTime CreatedUtc { get; set; }
        [Display(Name = "record age")]
        public int? CreatedAgeDays { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

        public Role()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
        { }

        public override Role Prepare(Role Model)
        {
            var ActivityService = ServiceFactory.CreateService<IActivityService>();
            Model.AllActivities = Mapper.Map<IList<Dtos.Activity>, IList<Activity>>(ActivityService.GetAll().OrderBy(a => a.Name).ToList());

            //var UserService = ServiceFactory.CreateService<IUserService>();
            //Model.AllUsers = Mapper.Map<IList<Dtos.User>, IList<User>>(UserService.GetAll().OrderBy(u => u.Fullname.SurnameFirstname).ToList());

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

        public override bool Save(Role Model)
        {
            using (new FunctionLogger(Log))
            {
                var RoleDto = Mapper.Map<Role, Dtos.Role>(Model);

                RoleDto.Activities = Model.SelectedActivities == null ? new List<Dtos.Activity>()
                : Mapper.Map<IList<Activity>, IList<Dtos.Activity>>(
                    (from a in Model.AllActivities
                     where (Model.SelectedActivities.Contains(a.Id))
                     select a).ToList());

                //RoleDto.Users = Model.SelectedUsers == null ? new List<Dtos.User>()
                //: Mapper.Map<IList<User>, IList<Dtos.User>>(
                //    (from a in Model.AllUsers
                //     where (Model.SelectedUsers.Contains(a.Id))
                //     select a).ToList());

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
