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

        Guid ChangeEmailAddressInitiate(Guid UserId, string NewEmailAddress, bool AssumeSent = false);
        User ChangeEmailAddressComplete(Guid ConfirmationId);

        User ChangeLocalPassword(Guid UserId, string OldPassword, string NewPassword);

        User ExpireLocalCredentials(Guid UserId);

        #region Administration
        User CreateAsAdministrator(Guid UserId, string Firstname, string Surname, string EmailAddress, string Password);
        User UpdateAsAdministrator(User User);
        User ResetLocalCredentialsAsAdministrator(Guid UserId, string EmailAddress, string Password);
        #endregion

        User GetByEmailAddress(string EmailAddress);

        Profile GetProfile(Guid UserId);
        void SetProfile(Profile Profile);
        Settings GetSettings(Guid UserId);

        IList<Role> GetRoles(Guid UserId);
        User SetRoles(Guid UserId, IList<Role> Roles);

        //IList<Activity> GetAllowedActivities(Guid UserId);      // Returns unique activities
        IList<Activity> GetDeniedActivities(Guid UserId);
        bool UserHasActivity(Guid UserId, string Name, string Area, string Action);   // TO DO: Doubt this is needed
    }
}
