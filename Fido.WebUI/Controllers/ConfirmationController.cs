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
    public class ConfirmationController : BaseController
    {
        //#region Profile
        //public ActionResult Profile_()
        //{
        //    return Dispatcher.ReturnLoadedModel<ProfileVM>(
        //        Id: AuthenticatedId,
        //        Result: m => View(m));
        //}

        //[HttpPost]
        //public ActionResult Profile_(ProfileVM Model)
        //{
        //    return Dispatcher.SavePostedModel(
        //        DataModel: Model,
        //        SuccessResult: m => View(),
        //        NonsuccessResult: m => View(m));
        //}

        //public ActionResult ProfileImage(Guid UserId)
        //{
        //    return Dispatcher.ReturnLoadedModel<ProfileImageVM>(
        //        Id: UserId,
        //        Result:
        //            m => m != null && m.Image != null
        //                ? new FileContentResult(m.Image, "image/jpeg")
        //                : null);
        //}
        //#endregion

        //#region Settings
        //public ActionResult Settings()
        //{
        //    return Dispatcher.ReturnLoadedModel<SettingsVM>(
        //        Id: AuthenticatedId,
        //        Result: m => View(m));
        //}
        //#endregion

        //#region Change Password
        //public ActionResult ChangePassword()
        //{
        //    return Dispatcher.ReturnEmptyModel(
        //        new ChangePasswordVM(),
        //        Result: m => PartialView());
        //}

        //[HttpPost]
        //public ActionResult ChangePassword(ChangePasswordVM Model)
        //{
        //    return Dispatcher.SavePostedModel(
        //        DataModel: Model,
        //        SuccessResult: m => ModalRedirectToLocal(Url.Action("Settings", "Account", new { Area = "" }, null)),
        //        NonsuccessResult: m => PartialView(m));
        //}
        //#endregion

        //#region Change Email Address
        //public ActionResult ChangeEmailAddress()
        //{
        //    return Dispatcher.ReturnEmptyModel(
        //        new ChangeEmailAddressVM(),
        //        Result: m => PartialView());
        //}

        //[HttpPost]
        //public ActionResult ChangeEmailAddress(ChangeEmailAddressVM Model)
        //{
        //    return Dispatcher.SavePostedModel(
        //        DataModel: Model,
        //        SuccessResult: m => ModalRedirectToLocal(Url.Action("Settings", "Account", new { Area = "" }, null)),
        //        NonsuccessResult: m => PartialView(m));
        //}
        //#endregion

        //#region Set Credentials
        //public ActionResult SetCredentials()
        //{
        //    return Dispatcher.ReturnEmptyModel(
        //        new SetCredentialsVM(),
        //        Result: m => PartialView());
        //}

        //[HttpPost]
        //public ActionResult SetCredentials(SetCredentialsVM Model)
        //{
        //    return Dispatcher.SavePostedModel(
        //        DataModel: Model,
        //        SuccessResult: m => ModalRedirectToLocal(Url.Action("Settings", "Account", new { Area = "" }, null)),
        //        NonsuccessResult: m => PartialView(m));
        //}
        //#endregion

        //#region Manage External Credentials
        //[HttpPost]
        //public ActionResult LinkExternalCredentials(string Provider)
        //{
        //    return new ChallengeResult(Provider, Url.Action("LinkExternalCredentialsCallback"), User.Identity.GetUserId());
        //}

        //public async Task<ActionResult> LinkExternalCredentialsCallback()
        //{
        //    var ExternalLoginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync(ChallengeResult.XsrfKey, User.Identity.GetUserId());

        //    if (ExternalLoginInfo == null)
        //    {
        //        Flash.Error("Corruption detected when reading the external credentials.");
        //        return RedirectToAction("Settings");
        //    }

        //    return Dispatcher.SavePostedModel(
        //        DataModel: new LinkExternalCredentialsCallbackVM
        //            {
        //                LoginProvider = ExternalLoginInfo.Login.LoginProvider,
        //                ProviderKey = ExternalLoginInfo.Login.ProviderKey,
        //                EmailAddress = ExternalLoginInfo.Email
        //            },
        //            SuccessResult: m => RedirectToAction("Settings"),
        //            NonsuccessResult: m => RedirectToAction("Settings"));
        //}

        //public ActionResult UnlinkExternalCredentials(UnlinkExternalCredentialsVM Model)
        //{
        //    return Dispatcher.SavePostedModel(
        //        DataModel: Model,
        //        SuccessResult: m => RedirectToAction("Settings"),
        //        NonsuccessResult: m => RedirectToAction("Settings"));
        //}
        //#endregion

        public ActionResult Confirmation(ConfirmationVM Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("LocalLogin", "Authentication"),
                NonsuccessResult: m => RedirectToAction("LocalLogin", "Authentication"));
        }
    }
}
