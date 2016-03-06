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
    public class ActivityVM : Model<ActivityVM>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public IList<RoleVM> AllRoles = new List<RoleVM>();

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

        public ActivityVM() { }
        public ActivityVM(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true,
                        RequiresWritePermission: true)
        { }

        public override ActivityVM Prepare(ActivityVM Model)
        {
            var RoleService = ServiceFactory.CreateService<IRoleService>();
            Model.AllRoles = Mapper.Map<IList<Dtos.Role>, IList<RoleVM>>(RoleService.GetAll().OrderBy(r => r.Name).ToList());

            return Model;
        }

        public override ActivityVM Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var ActivityService = ServiceFactory.CreateService<IActivityService>();

                var Activity = ActivityService.Get(Id);
                var Model = Mapper.Map<Dtos.Activity, ActivityVM>(Activity);

                return Model;
            }
        }

        public override bool Save(ActivityVM Model)
        {
            using (new FunctionLogger(Log))
            {
                var ActivityDto = Mapper.Map<ActivityVM, Dtos.Activity>(Model);

         //       ActivityDto.Roles = new List<Dtos.Role>();
                ActivityDto.Roles = Model.SelectedRoles == null ? new List<Dtos.Role>()
                : Mapper.Map<IList<RoleVM>, IList<Dtos.Role>>(
                    (from a in Model.AllRoles
                     where (Model.SelectedRoles.Contains(a.Id))
                     select a).ToList());

                var ActivityService = ServiceFactory.CreateService<IActivityService>();
                ActivityDto = ActivityService.Save(ActivityDto);

                FeedbackAPI.DisplaySuccess("The activity details have been saved");
                return true;
            }
        }
    }
}
