using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Dtos;

namespace Fido.Service
{
    public interface IRoleService : ICRUDService<Role>
    {
        IList<Role> GetPageInDefaultOrder(char SortOrder, int Skip, int Take, string Filter);
        IList<Role> GetPageInNameOrder(char SortOrder, int Skip, int Take, string Filter);

        Role GetByName(string Name);
        IList<User> GetUsersInRole(Guid RoleId);
        IList<Activity> GetActivitiesForRole(Guid RoleId);
        void SetActivitiesForRole(Guid RoleId, IList<Activity> Activities);
        void SetAdministrationRole(string RoleName);

        bool NameFree(string Name);             // Name must be unique
    }
}
