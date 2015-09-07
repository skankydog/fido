using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Core;
using Fido.Dtos;
using Fido.Service.Implementation;

namespace Fido.Service
{
    public interface IUserService : ICRUDService<User>
    {
        IList<User> GetPageInSurnameOrder(int PageNo, int PageSize);

        Guid InitiateChangeEmailAddress(Guid UserId, string NewEmailAddress);
        User CompleteChangeEmailAddress(Guid ConfirmationId);

        User ChangeLocalPassword(Guid UserId, string OldPassword, string NewPassword);

        #region Administration
        User Update(User User);
        #endregion

        User GetByEmailAddress(string EmailAddress);

        Profile GetProfile(Guid UserId);
        void SetProfile(Profile Profile);
        Settings GetSettings(Guid UserId);

        IList<Role> GetRoles(Guid UserId);
        void SetRoles(Guid UserId, IList<Role> Roles);

        IList<Activity> GetActivities(Guid UserId);      // Returns unique activities
        bool UserHasActivity(Guid UserId, string ActivityName);
    }
}
