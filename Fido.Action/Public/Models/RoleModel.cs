using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.Action.Models
{
    public class RoleModel : Model<RoleModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public IList<ActivityModel> AllActivities = new List<ActivityModel>();
        public IList<UserModel> AllUsers = new List<UserModel>();

        public Guid Id { get; set; }

        [Display(Name = "role name")]
        [Required(ErrorMessage = "The role name field cannot be left blank")]
        public string Name { get; set; }

        public IList<Guid> SelectedActivities { get; set; }
        public IList<Guid> SelectedUsers { get; set; }

        [Display(Name = "created date")]
        public DateTime CreatedUtc { get; set; }
        [Display(Name = "record age")]
        public int? CreatedAgeDays { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

        public RoleModel()
        {
            Id = Guid.NewGuid();
            CreatedUtc = DateTime.UtcNow;
            IsNew = true;
        }

        public RoleModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true,
                        RequiresWritePermission: true)
        { }

        public override RoleModel Prepare(RoleModel Model)
        {
            var ActivityService = ServiceFactory.CreateService<IActivityService>();
            Model.AllActivities = Mapper.Map<IList<Dtos.Activity>, IList<ActivityModel>>(ActivityService.GetAll().OrderBy(a => a.Name).ToList());

            var UserService = ServiceFactory.CreateService<IUserService>();
            Model.AllUsers = Mapper.Map<IList<Dtos.User>, IList<UserModel>>(UserService.GetAll().OrderBy(u => u.Fullname.SurnameFirstname).ToList());

            return Model;
        }

        public override RoleModel Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var RoleService = ServiceFactory.CreateService<IRoleService>();

                var Role = RoleService.Get(Id);
                var Model = Mapper.Map<Dtos.Role, RoleModel>(Role);

                return Model;
            }
        }

        public override bool Write(RoleModel Model)
        {
            using (new FunctionLogger(Log))
            {
                var RoleDto = Mapper.Map<RoleModel, Dtos.Role>(Model);

                RoleDto.Activities = new List<Dtos.Activity>();
                RoleDto.Activities = Model.SelectedActivities == null ? new List<Dtos.Activity>()
                : Mapper.Map<IList<ActivityModel>, IList<Dtos.Activity>>(
                    (from a in Model.AllActivities
                     where (Model.SelectedActivities.Contains(a.Id))
                     select a).ToList());

                RoleDto.Users = new List<Dtos.User>();
                RoleDto.Users = Model.SelectedUsers == null ? new List<Dtos.User>()
                : Mapper.Map<IList<UserModel>, IList<Dtos.User>>(
                    (from a in Model.AllUsers
                     where (Model.SelectedUsers.Contains(a.Id))
                     select a).ToList());

                var RoleService = ServiceFactory.CreateService<IRoleService>();
                RoleDto = RoleService.Save(RoleDto);

                FeedbackAPI.DisplaySuccess("The role details have been saved");
                return true;
            }
        }

        public override bool Delete(RoleModel Model)
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
