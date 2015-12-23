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
        IList<User> GetPageInDefaultOrder(char SortOrder,int Skip, int Take, string Filter);
        IList<User> GetPageInFirstnameOrder(char SortOrder,int Skip, int Take, string Filter);
        IList<User> GetPageInSurnameOrder(char SortOrder,int Skip, int Take, string Filter);
        IList<User> GetPageInEmailAddressOrder(char SortOrder,int Skip, int Take, string Filter);
        IList<User> GetPageInLocalCredentialOrder(char SortOrder,int Skip, int Take, string Filter);
        IList<User> GetPageInExternalCredentialOrder(char SortOrder, int Skip, int Take, string Filter);

        Guid InitiateChangeEmailAddress(Guid UserId, string NewEmailAddress);
        User CompleteChangeEmailAddress(Guid ConfirmationId);

        User ChangeLocalPassword(Guid UserId, string OldPassword, string NewPassword);

        #region Administration
        User SetLocalCredentialState(Guid UserId, string State);
        User SetExternalCredentialState(Guid UserId, string State);
        #endregion

        User GetByEmailAddress(string EmailAddress);

        Profile GetProfile(Guid UserId);
        void SetProfile(Profile Profile);
        Settings GetSettings(Guid UserId);

        IList<Role> GetRoles(Guid UserId);
        User SetRoles(Guid UserId, IList<Role> Roles);

        IList<Activity> GetActivities(Guid UserId);      // Returns unique activities
        bool UserHasActivity(Guid UserId, string ActivityName);
    }
}
