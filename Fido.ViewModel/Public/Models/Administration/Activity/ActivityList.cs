// http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.ViewModel.Implementation;

namespace Fido.ViewModel.Models.Administration
{
    public class ActivityList : Model<ActivityList>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public string sEcho;
        public int iTotalRecords;
        public int iTotalDisplayRecords;
        public IList<string[]> aaData = new List<string[]>();
        #endregion

        public ActivityList()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.NA)
        { }

        public override ActivityList Read(ListOptions ListOptions)
        {
            using (new FunctionLogger(Log))
            {
                var PageOfRecords = GetPageOfRecords(ListOptions.SortColumn, ListOptions.SortOrder, ListOptions.Skip, ListOptions.Take, ListOptions.Filter);
                var CountUnfiltered = CountAll();
                var CountFiltered = ListOptions.Filter.IsNullOrEmpty() ? CountUnfiltered : PageOfRecords.Count();

                return new ActivityList
                {
                    sEcho = ListOptions.Echo,
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
                IList<Dtos.Activity> ActivityDtos;

                switch (SortColumn)
                {
                    case 0:
                        ActivityDtos = ActivityService.GetPageInAreaOrder(SortOrder, Skip, Take, Filter);
                        break;

                    case 1:
                        ActivityDtos = ActivityService.GetPageInNameOrder(SortOrder, Skip, Take, Filter);
                        break;

                    case 2:
                        ActivityDtos = ActivityService.GetPageInReadWriteOrder(SortOrder, Skip, Take, Filter);
                        break;

                    default:
                        ActivityDtos = ActivityService.GetPageInDefaultOrder(SortOrder, Skip, Take, Filter);
                        break;
                }

                return (from ActivityDto in ActivityDtos
                        select new[] {
                            ActivityDto.Area.Nvl(),
                            ActivityDto.Name.Nvl(),
                            ActivityDto.ReadWrite.Nvl(),
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
