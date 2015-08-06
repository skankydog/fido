using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action.Models;

namespace Fido.WebUI.Controllers
{
    [RequireHttps]
    [AllowAnonymous]
    public class AuthenticationController : BaseController
    {
        #region Properties, Constructors & Dispose

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        //private OOTBUserManager Manager;

        //public AuthenticationController()
        //{
        //}

        //public AuthenticationController(OOTBUserManager Manager)
        //{
        //    UserManager = Manager;
        //}

        //public OOTBUserManager UserManager
        //{
        //    get
        //    {
        //        return Manager ?? HttpContext.GetOwinContext().GetUserManager<OOTBUserManager>();
        //    }
        //    private set
        //    {
        //        Manager = value;
        //    }
        //}



        //protected override void Dispose(bool Disposing)
        //{
        //    if (Disposing && UserManager != null)
        //    {
        //        UserManager.Dispose();
        //        UserManager = null;
        //    }

        //    base.Dispose(Disposing);
        //}
        #endregion

        #region Local Login
        public ActionResult LocalLogin(string ReturnUrl = @"/Home/Index")
        {
            ViewBag.ReturnUrl = ReturnUrl;

            // As this is the login, we just need to display the view.
            return View();
        }

        [HttpPost]
        public ActionResult LocalLogin(LocalLoginModel Model, string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;

            return Dispatcher.Write(
                Model: Model,
                SuccessUI: m => RedirectToLocal(ReturnUrl),
                FailureUI: m => View(),
                InvalidUI: m => View(m));
        }
        #endregion

        #region External Login
        [HttpPost]
        public ActionResult ExternalLogin(string Provider, string ReturnUrl)
        {
            return new ChallengeResult(Provider, Url.Action("ExternalLoginCallback", "Authentication", new { ReturnUrl = ReturnUrl }));
        }

        public ActionResult ExternalLoginCallback(string ReturnUrl)
        {
            // var ExternalLoginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            var ExternalLoginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();

            if (ExternalLoginInfo == null)
            {
                Flash.Error("Invalid external credential details");
                return RedirectToAction("LocalLogin");
            }

            return Dispatcher.Write(
                Model: new ExternalLoginCallbackModel
                    {
                        LoginProvider = ExternalLoginInfo.Login.LoginProvider,
                        ProviderKey = ExternalLoginInfo.Login.ProviderKey,
                        EmailAddress = ExternalLoginInfo.Email,
                        Name = ExternalLoginInfo.ExternalIdentity.Name
                    },
                SuccessUI: m => RedirectToLocal(ReturnUrl),
                FailureUI: m => RedirectToAction("LocalLogin"),
                InvalidUI: m => RedirectToAction("LocalLogin"));
        }
        #endregion

        #region Logout
        [HttpPost]
        [Authorize]
        public ActionResult Logout()
        {
            // As this is the logout, there is no VM. The generation of
            // the view does not need service layer functions.
            SignOut();
            LoggedInCredentialState = "None";
            Flash.Success("You have successfully logged out.");

            return RedirectToAction("LocalLogin");
        }
        #endregion

        #region Registration
        public ActionResult Registration()
        {
            return Dispatcher.View<RegistrationModel>(View);
        }

        [HttpPost]
        public ActionResult Registration(RegistrationModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                SuccessUI: m => RedirectToAction("LocalLogin"),
                FailureUI: m => View(m),
                InvalidUI: m => View(m));
        }
        #endregion

        #region Forgotten Password
        public ActionResult ForgottenPassword()
        {
            return Dispatcher.View<ForgottenPasswordModel>(View);
        }

        [HttpPost]
        public ActionResult ForgottenPassword(ForgottenPasswordModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                SuccessUI: m => RedirectToAction("LocalLogin"),
                FailureUI: m => View(m),
                InvalidUI: m => View(m));
        }

        public ActionResult ResetPassword(Guid ConfirmationId)
        {
            return Dispatcher.View<ResetPasswordModel>(View);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                SuccessUI: m => RedirectToAction("Index", "Home"),
                FailureUI: m => View(m),
                InvalidUI: m => View(m));
        }
        #endregion
    }
}
