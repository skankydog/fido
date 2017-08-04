using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Service.Exceptions;
using Fido.Core;
using Fido.Dtos;
using Fido.DataAccess;

namespace Fido.Service.Implementation
{
    internal class RoleService : CRUDService<Role, Entities.Role, DataAccess.IRoleRepository>, IRoleService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Pages
        public IList<Role> GetPageInDefaultOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Id),
                OrderByDescending: q => q.OrderByDescending(s => s.Id));
        }

        public IList<Role> GetPageInNameOrder(char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Name),
                OrderByDescending: q => q.OrderByDescending(s => s.Name));
        }

        private IList<Role> GetPage(char SortOrder, int Skip, int Take, string Filter,
            Func<IQueryable<Entities.Role>, IOrderedQueryable<Entities.Role>> OrderByAscending,
            Func<IQueryable<Entities.Role>, IOrderedQueryable<Entities.Role>> OrderByDescending)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var RoleRepository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
                    var OrderBy = SortOrder == 'a' ? OrderByAscending : OrderByDescending;
                    var Query = RoleRepository.GetAsIQueryable(e => e.Id != null, OrderBy);

                    if (Filter.IsNotNullOrEmpty())
                    {
                        Query = Query.Where(e => e.Name.ToLower().Contains(Filter.ToLower()));
                    }

                    Query = Query.Skip(Skip).Take(Take);

                    var EntityList = Query.ToList(); // Hit the database

                    IList<Role> DtoList = AutoMapper.Mapper.Map<IList<Entities.Role>, IList<Role>>(EntityList);
                    return DtoList;
                }
            }
        }
        #endregion

        public Role GetByName(string Name)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
                    var RoleEntity = Repository.Get(e => e.Name == Name);

                    return Mapper.Map<Entities.Role, Role>(RoleEntity);
                }
            }
        }

        public IList<User> GetUsersInRole(Guid RoleId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var UserRepository = DataAccessFactory.CreateRepository<DataAccess.IUserRepository>(UnitOfWork);
                    var UsersInRole = (from u in UserRepository.GetAsIQueryable(e => e.Id != Guid.Empty)
                                       from r in u.Roles where r.Id == RoleId
                                       select u).ToList();

                    IList<User> UserDTOs = null;
                    UserDTOs = Mapper.Map<IList<Entities.User>, IList<User>>(UsersInRole, UserDTOs);

                    return UserDTOs;
                }
            }
        }

        public IList<Activity> GetActivitiesForRole(Guid RoleId)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    IRoleRepository Repository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
                    Entities.Role RoleEntity = Repository.Get(e => e.Id == RoleId);

                    IList<Activity> Activities = new List<Activity>();
                    Activities = Mapper.Map<ICollection<Entities.Activity>, IList<Dtos.Activity>>(RoleEntity.Activities, Activities);

                    return Activities;
                }
            }
        }

        public void SetActivitiesForRole(Guid RoleId, IList<Activity> Activities)
        {
            using (new FunctionLogger(Log))
            {
                var ActivityIds =
                    from Activity A in Activities
                    where A.IsNew == false
                    select A.Id;

                if (ActivityIds.Count() != Activities.Count())
                    throw new ArgumentException("Role.Activities");

                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    IActivityRepository ActivityRepository = DataAccessFactory.CreateRepository<IActivityRepository>(UnitOfWork);
                    IRoleRepository RoleRepository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);

                    var ExistingActivities = ActivityRepository.GetAsIQueryable(e => e.Id != Guid.Empty).Where(e => ActivityIds.Contains(e.Id));

                    // Now read, update and save the role entity with the activities. When we read in the role, we need to eagerly
                    // read the activities as well - otherwise, when we write back to the database, EF will see the activities as
                    // inserts only, not as a change to the activities within the role
                    var RoleEntity = RoleRepository.Get(e => e.Id == RoleId);

                    RoleEntity.Activities = ExistingActivities.ToList();

                    RoleRepository.Update(RoleEntity);
                    UnitOfWork.Commit();
                }
            }
        }

        public void SetAdministrationRole(string RoleName)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var RoleRepository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
                    var AdministratorRole = RoleRepository.Get(e => e.Name == RoleName);

                    if (AdministratorRole == null)
                    {
                        AdministratorRole = new Entities.Role { Name = RoleName };
                        //AdministratorRole = RoleRepository.InsertWithChildren(AdministratorRole);
                        AdministratorRole = RoleRepository.Insert(AdministratorRole);
                        UnitOfWork.Commit();

                        AdministratorRole = RoleRepository.Get(e => e.Name == RoleName);
                    }

                    var ActivityRepository = DataAccessFactory.CreateRepository<IActivityRepository>(UnitOfWork);
                    var AllActivities = ActivityRepository.GetAsIEnumerable(e => e.Id != Guid.Empty).ToList();

                    AdministratorRole.Activities = AllActivities;
                    RoleRepository.Update(AdministratorRole);

                    UnitOfWork.Commit();
                }
            }
        }

        public bool NameFree(string Name)
        {
            using (new FunctionLogger(Log))
            {
                if (GetByName(Name) == null)
                    return true;

                return false;
            }
        }
    }
}
