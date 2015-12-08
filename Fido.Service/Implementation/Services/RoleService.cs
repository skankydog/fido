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
                                       from tag in u.Roles where tag.Id == RoleId
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
