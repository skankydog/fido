using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using Owin.Security.Providers.LinkedIn;
using Owin;
using System;

namespace Fido.WebUI
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864

        public void ConfigureAuth(IAppBuilder App)
        {
            // Configure the db context and user manager to use a single instance per request
   //         App.CreatePerOwinContext(ApplicationDbContext.Create);
   //         App.CreatePerOwinContext<OOTBUserManager>(OOTBUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user and to use a cookie
            // to temporarily store information about a user logging in with a third party login provider Configure
            // the sign in cookie...
            App.UseCookieAuthentication(
                new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Authentication/LocalLogin"), // Should put this in the web.config
                    Provider = new CookieAuthenticationProvider
                    {
                        OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                            validateInterval: TimeSpan.FromMinutes(30),
                            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                    }
                });
            
            App.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //App.UseMicrosoftAccountAuthentication(
            //    clientId: "to do",
            //    clientSecret: "to do");

            App.UseTwitterAuthentication(
               consumerKey: "Rik3RCeRhN2HVa60xuiIWIkNg",
               consumerSecret: "z466zYzIjU0Lq5jPDrnTREujPOKQV1I4dhxWSCTdtjoTD2ZBj2");

            var FacebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions();
            FacebookOptions.AppId = "1387976031522621";
            FacebookOptions.AppSecret = "e5229c4cbdbf8f5a8449ba45c44f0d87";
            FacebookOptions.Scope.Add("email");
            FacebookOptions.SignInAsAuthenticationType = Microsoft.Owin.Security.AppBuilderSecurityExtensions.GetDefaultSignInAsAuthenticationType(App);
            App.UseFacebookAuthentication(FacebookOptions);

            App.UseGoogleAuthentication(
                new GoogleOAuth2AuthenticationOptions()
                {
                    ClientId = "752114926788-iii5pm8340seupqve43r0jfbhas8hjnu.apps.googleusercontent.com",
                    ClientSecret = "h1NVK8XUa7yB4sXmIE_fX5Ru"
                });

            App.UseLinkedInAuthentication(
                "75tz53v5ic4g6x",
                "biUKrNflO16VVCGl");
        }

        //private FacebookAuthenticationOptions FacebookOptions
        //{
        //    get
        //    {
        //        var FacebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions()
        //        {
        //            AppId = "1387976031522621",
        //            AppSecret = "e5229c4cbdbf8f5a8449ba45c44f0d87",

        //            //Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
        //            //{
        //            //    OnAuthenticated = (Context) =>
        //            //        {
        //            //            var RawUserObjectFromFacebookAsJson = Context.User; // All data in here

        //            //            // Only some of the basic details from facebook  like id, username, email etc
        //            //            // are added as claims. But you can retrieve any other details from this raw
        //            //            // json object and add it as claims here. Subsequently adding a claim here will
        //            //            // also send this claim as part of the cookie set on the browser so you can
        //            //            // retrieve on every successive request. 
        //            //            Context.Identity.AddClaim(...);

        //            //            return Task.FromResult(0);
        //            //        }
        //            //}
        //        };

        //        //Way to specify additional scopes
        //        FacebookOptions.Scope.Add("email");
        //        FacebookOptions.Scope.Add("user_birthday");
        //   //     FacebookOptions.SignInAsAuthenticationType = Microsoft.Owin.Security.AppBuilderSecurityExtensions.GetDefaultSignInAsAuthenticationType(App);

        //        return FacebookOptions;
        //    }
        //}
    }
}
