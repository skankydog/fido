using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Fido.WebUI.Models;

namespace Fido.WebUI
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class MyUserStore : IUserStore<MyUser, Guid>, IUserLoginStore<MyUser, Guid>
    {
        #region IUserStore Implementations
        public Task CreateAsync(MyUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(MyUser user)
        {
            throw new NotImplementedException();
        }

        public Task<MyUser> FindByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<MyUser> FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(MyUser user)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IUserLoginStore Implementations
        public Task AddLoginAsync(MyUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<MyUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(MyUser user)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(MyUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
