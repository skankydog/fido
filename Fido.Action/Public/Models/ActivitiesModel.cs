// http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models
{
    public class ActivitiesModel : Model<ActivitiesModel>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public string sEcho;
        public int iTotalRecords;
        public int iTotalDisplayRecords;
        public IList<string[]> aaData = new List<string[]>();
        #endregion

        public ActivitiesModel() { }
        public ActivitiesModel(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true,
                        RequiresWritePermission: true)
        { }

        public override ActivitiesModel Read(IndexOptions IndexOptions)
        {
            using (new FunctionLogger(Log))
            {
                var PageOfRecords = GetPageOfRecords(IndexOptions.SortColumn, IndexOptions.SortOrder, IndexOptions.Skip, IndexOptions.Take, IndexOptions.Filter);
                var CountUnfiltered = CountAll();
                var CountFiltered = IndexOptions.Filter.IsNullOrEmpty() ? CountUnfiltered : PageOfRecords.Count();

                return new ActivitiesModel
                {
                    sEcho = IndexOptions.Echo,
                    iTotalRecords = CountUnfiltered,
                    iTotalDisplayRecords = CountFiltered,
                    aaData = PageOfRecords
                };
            }
        }

        private IList<string[]> GetPageOfRecords(int SortColumn, char SortOrder, int Skip, int Take, string Filter)
        {
            using (new FunctionLogger(Log))
            {
                var ActivityService = ServiceFactory.CreateService<IActivityService>();
                IList<Activity> ActivityDtos;

                switch (SortColumn)
                {
                    case 0:
                        ActivityDtos = ActivityService.GetPageInNameOrder(SortOrder, Skip, Take, Filter);
                        break;

                    default:
                        ActivityDtos = ActivityService.GetPageInDefaultOrder(SortOrder, Skip, Take, Filter);
                        break;
                }

                return (from ActivityDto in ActivityDtos
                        select new[] {
                        ActivityDto.Name.Nvl(),
                        ActivityDto.Id.ToString(), // Edit
                        ActivityDto.Id.ToString()  // Delete
                    }).ToArray();
            }
        }

        private int CountAll()
        {
            using (new FunctionLogger(Log))
            {
                var ActivityService = ServiceFactory.CreateService<IActivityService>();
                return ActivityService.CountAll();
            }
        }
    }
}
