using AutoMapper;
//using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Core;
using Fido.Dtos;
using Fido.DataAccess;

namespace Fido.Service.Implementation
{
    internal class ActivityService : CRUDService<Dtos.Activity, Entities.Activity, IActivityRepository>, IActivityService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Pages
        public IList<Activity> GetPageInDefaultOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPageInNameOrder(SortOrder, Skip, Take, Filter);
        }

        public IList<Activity> GetPageInNameOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Name),
                OrderByDescending: q => q.OrderByDescending(s => s.Name));
        }

        public IList<Activity> GetPageInAreaOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Area),
                OrderByDescending: q => q.OrderByDescending(s => s.Area));
        }

        public IList<Activity> GetPageInReadWriteOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.ReadWrite),
                OrderByDescending: q => q.OrderByDescending(s => s.ReadWrite));
        }

        private IList<Activity> GetPage(char SortOrder, int Skip, int Take, string Filter,
            Func<IQueryable<Entities.Activity>, IOrderedQueryable<Entities.Activity>> OrderByAscending,
            Func<IQueryable<Entities.Activity>, IOrderedQueryable<Entities.Activity>> OrderByDescending)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var ActivityRepository = DataAccessFactory.CreateRepository<IActivityRepository>(UnitOfWork);
                    var OrderBy = SortOrder == 'a' ? OrderByAscending : OrderByDescending;
                    var Query = ActivityRepository.GetAsIQueryable(e => e.Id != null, OrderBy);

                    if (Filter.IsNotNullOrEmpty())
                    {
                        Query = Query.Where(e => e.Name.ToLower().Contains(Filter.ToLower())
                            || e.ReadWrite.ToLower().Contains(Filter.ToLower())
                            || e.Area.ToLower().Contains(Filter.ToLower()));
                    }

                    Query = Query.Skip(Skip).Take(Take);

                    var EntityList = Query.ToList(); // hit the database

                    IList<Activity> DtoList = AutoMapper.Mapper.Map<IList<Entities.Activity>, IList<Activity>>(EntityList);
                    return DtoList;
                }
            }
        }
        #endregion

        public Activity Get(string Area, string Name, string ReadWrite)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IActivityRepository>(UnitOfWork);
                    var ActivityEntity = Repository.Get(e => e.Area == Area && e.Name == Name && e.ReadWrite == ReadWrite);

                    var ActivityDTO = Mapper.Map<Entities.Activity, Activity>(ActivityEntity);

                    return ActivityDTO;
                }
            }
        }
    }
}
