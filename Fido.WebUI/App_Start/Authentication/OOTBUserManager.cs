using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Fido.WebUI.Models;

namespace Fido.WebUI
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class OOTBUserManager : UserManager<OOTBUser>
    {
        public OOTBUserManager(IUserStore<OOTBUser> Store)
            : base(Store)
        {
        }

        public static OOTBUserManager Create(IdentityFactoryOptions<OOTBUserManager> Options, IOwinContext Context) 
        {
            var Manager = new OOTBUserManager(new UserStore<OOTBUser>(Context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            Manager.UserValidator = new UserValidator<OOTBUser>(Manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            Manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            Manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<OOTBUser>
            {
                MessageFormat = "Your security code is: {0}"
            });

            Manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<OOTBUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });

            Manager.EmailService = new EmailService();
            Manager.SmsService = new SmsService();

            var DataProtectionProvider = Options.DataProtectionProvider;
            
            if (DataProtectionProvider != null)
            {
                Manager.UserTokenProvider = new DataProtectorTokenProvider<OOTBUser>(DataProtectionProvider.Create("ASP.NET Identity"));
            }

            return Manager;
        }
    }
}
