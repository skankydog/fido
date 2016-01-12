using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.Action.Models
{
    public class ActivityModel : Model<ActivityModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "created date")]
        public DateTime CreatedUtc { get; set; }
        [Display(Name = "record age")]
        public int? CreatedAgeDays { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

        public ActivityModel()
        {
            Id = Guid.NewGuid();
            CreatedUtc = DateTime.UtcNow;
            IsNew = true;
        }

        public ActivityModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true,
                        RequiresWritePermission: true)
        { }

        //public override ActivityModel Read(Guid Id)
        //{
        //    using (new FunctionLogger(Log))
        //    {
        //        var ActivityService = ServiceFactory.CreateService<IActivityService>();

        //        var Activity = ActivityService.Get(Id);
        //        var Model = Mapper.Map<Dtos.Activity, ActivityModel>(Activity);

        //        return Model;
        //    }
        //}
    }
}
