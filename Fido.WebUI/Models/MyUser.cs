using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Fido.WebUI;

namespace Fido.WebUI.Models
{
    public class MyUser : IUser<Guid>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(MyUserManager Manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var UserIdentity = await Manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here

            return UserIdentity;
        } 
     
        public Guid Id { get; set; }
        public string UserName { get; set; } // EmailAddress
        public IList<UserLoginInfo> Logins { get; set; }

        //// can also define optional properties such as:
        ////    PasswordHash
        ////    SecurityStamp
        ////    Claims
        ////    Logins
        ////    Roles
    }
}
