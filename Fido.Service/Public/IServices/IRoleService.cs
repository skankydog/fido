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
        Role GetByName(string Name);
        IList<User> GetUsersInRole(Guid RoleId);
        IList<Activity> GetActivitiesForRole(Guid RoleId);
        void SetActivitiesForRole(Guid RoleId, IList<Activity> Activities);
        void SetAdministrationRole(string RoleName);

        bool NameFree(string Name);             // Name must be unique
    }
}
