using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Fido.WebUI.Models;

namespace Fido.WebUI
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class MyUserManager : UserManager<MyUser, Guid>
    {
        public MyUserManager(IUserStore<MyUser, Guid> Store)
            : base(Store)
        {
        }

        public static MyUserManager Create(IdentityFactoryOptions<MyUserManager> Options, IOwinContext Context) 
        {
            var Manager = new MyUserManager(new MyUserStore());

            // Configure validation logic for usernames
            Manager.UserValidator = new UserValidator<MyUser, Guid>(Manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            Manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
     //           RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            Manager.EmailService = new EmailService();
            Manager.SmsService = new SmsService();

            var DataProtectionProvider = Options.DataProtectionProvider;
            
            if (DataProtectionProvider != null)
            {
                Manager.UserTokenProvider = new DataProtectorTokenProvider<MyUser, Guid>(DataProtectionProvider.Create("ASP.NET Identity"));
            }

            return Manager;
        }
    }
}
