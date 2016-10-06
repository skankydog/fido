using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action.Models.Authentication;
using Fido.WebUI.Common;

namespace Fido.WebUI.Areas.Authentication.Controllers
{
    [RequireHttps]
    [AllowAnonymous]
    public class ExternalLoginController : BaseController
    {
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        [HttpPost]
        public ActionResult Create(string Provider, string ReturnUrl)
        {
            return new ChallengeResult(Provider, Url.Action("ExternalLoginCallback", "ExternalLogin", new { ReturnUrl = ReturnUrl, Area ="Authentication" }));
        }

        public ActionResult ExternalLoginCallback(string ReturnUrl)
        {
            // var ExternalLoginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            var ExternalLoginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();

            if (ExternalLoginInfo == null)
            {
                Flash.Error("Invalid external credential details");
                return RedirectToAction("Create", "Login", new { Area = "Authentication" });
            }

            return Dispatcher.Update(
                DataModel: new ExternalLoginCallback
                    {
                        LoginProvider = ExternalLoginInfo.Login.LoginProvider,
                        ProviderKey = ExternalLoginInfo.Login.ProviderKey,
                        EmailAddress = ExternalLoginInfo.Email,
                        Name = ExternalLoginInfo.ExternalIdentity.Name
                    },
                SuccessResult: m => RedirectToLocal(ReturnUrl),
                InvalidResult: m => RedirectToAction("Create", "Login"));
        }
    }
}
