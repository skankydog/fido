using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action.Models;
using Fido.WebUI.Common;

namespace Fido.WebUI.Controllers
{
    [RequireHttps]
    [AllowAnonymous]
    public class AuthenticationController : BaseController
    {
        #region Local Login
        public ActionResult LocalLogin(string ReturnUrl = @"/Home/Index")
        {
            ViewBag.ReturnUrl = ReturnUrl;

            // As this is the login, we just need to display the view.
            return View();
        }

        [HttpPost]
        public ActionResult LocalLogin(LocalLogin Model, string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;

            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => RedirectToLocal(ReturnUrl),
                InvalidResult: m => View());
        }
        #endregion

        #region External Login
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

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

            return Dispatcher.SavePostedModel(
                DataModel: new ExternalLoginCallback
                    {
                        LoginProvider = ExternalLoginInfo.Login.LoginProvider,
                        ProviderKey = ExternalLoginInfo.Login.ProviderKey,
                        EmailAddress = ExternalLoginInfo.Email,
                        Name = ExternalLoginInfo.ExternalIdentity.Name
                    },
                SuccessResult: m => RedirectToLocal(ReturnUrl),
                InvalidResult: m => RedirectToAction("LocalLogin"));
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
            return Dispatcher.ReturnEmptyModel(
                new RegistrationInitiate(),
                Result: m => View());
        }

        [HttpPost]
        public ActionResult Registration(RegistrationInitiate Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("LocalLogin"),
                InvalidResult: m => View(m));
        }
        #endregion

        //#region Forgotten/Reset Password
        //public ActionResult ForgottenPassword()
        //{
        //    return Dispatcher.ReturnEmptyModel/*<ForgottenPasswordModel>*/(
        //        new ForgottenPasswordInitiate(),
        //        Result: m => View());
        //}

        //[HttpPost]
        //public ActionResult ForgottenPassword(ForgottenPasswordInitiate Model)
        //{
        //    return Dispatcher.SavePostedModel(
        //        DataModel: Model,
        //        SuccessResult: m => RedirectToAction("LocalLogin"),
        //        InvalidResult: m => View(m));
        //}

        //public ActionResult ResetPassword(Guid ConfirmationId)
        //{
        //    return Dispatcher.ReturnEmptyModel(
        //        DataModel: new ForgottenPasswordComplete(),
        //        Result: m => View());
        //}

        //[HttpPost]
        //public ActionResult ResetPassword(ForgottenPasswordComplete Model)
        //{
        //    return Dispatcher.SavePostedModel(
        //        DataModel: Model,
        //        SuccessResult: m => RedirectToAction("Index", "Home"),
        //        InvalidResult: m => PartialView(m));
        //}
        //#endregion
    }
}
