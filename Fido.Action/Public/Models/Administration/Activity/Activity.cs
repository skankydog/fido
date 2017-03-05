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
    public class Activity : Model<Activity>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public IList<Role> AllRoles = new List<Role>();

        public string Area { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public string FullQualification { get; set; }

        public IList<Guid> SelectedRoles { get; set; }
        #endregion

        public Activity()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
        { }

        public override Activity Prepare(Activity Model)
        {
            var RoleService = ServiceFactory.CreateService<IRoleService>();
            Model.AllRoles = Mapper.Map<IList<Dtos.Role>, IList<Role>>(RoleService.GetAll().OrderBy(r => r.Name).ToList());

            return Model;
        }

        public override Activity Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var ActivityService = ServiceFactory.CreateService<IActivityService>();

                var Activity = ActivityService.Get(Id);
                var Model = Mapper.Map<Dtos.Activity, Activity>(Activity);

                return Model;
            }
        }

        public override bool Save(Activity Model)
        {
            using (new FunctionLogger(Log))
            {
                var ActivityService = ServiceFactory.CreateService<IActivityService>();
                var ActivityDto = ActivityService.Get(Model.Id);

                ActivityDto.Roles = Model.SelectedRoles == null ? new List<Dtos.Role>()
                : Mapper.Map<IList<Role>, IList<Dtos.Role>>(
                    (from a in Model.AllRoles
                     where (Model.SelectedRoles.Contains(a.Id))
                     select a).ToList());

                ActivityDto = ActivityService.Save(ActivityDto);

                FeedbackAPI.DisplaySuccess("The activity details have been saved");
                return true;
            }
        }

        public static IList<string> DeniedList(Guid UserId)
        {
            var List = new List<string>();

            if (UserId != Guid.Empty)
            {
                var UserService = ServiceFactory.CreateService<IUserService>();
                var Activities = UserService.GetDeniedActivities(UserId);
                var ActivityDtos = (from Dtos.Activity a in Activities
                                    select a).ToList();

                foreach (var Activity in ActivityDtos)
                {
                    List.Add(string.Concat(Activity.Action, ".", Activity.Name, ".", Activity.Area));
                }
            }

            return List;
        }
    }
}
