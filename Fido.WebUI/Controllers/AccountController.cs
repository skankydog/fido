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
    public class AccountController : BaseController
    {
        #region Profile
        public ActionResult Profile_()
        {
            return Dispatcher.Read<ProfileModel>(
                Id: AuthenticatedId,
                Success: m => View(m));
        }

        [HttpPost]
        public ActionResult Profile_(ProfileModel Model)
        {
            return Dispatcher.Write<ProfileModel>(
                Model: Model,
                Any: m => View(m));
        }

        public ActionResult ProfileImage(Guid UserId)
        {
            return Dispatcher.Read<ProfileImageModel>(
                Id: UserId,
                Success:
                    m => m != null && m.Image != null
                        ? new FileContentResult(m.Image, "image/jpeg")
                        : null);
        }
        #endregion

        #region Settings
        public ActionResult Settings()
        {
            return Dispatcher.Read<SettingsModel>(
                Id: AuthenticatedId,
                Success: m => View(m));
        }
        #endregion

        #region Change Password
        public ActionResult ChangePassword()
        {
            return Dispatcher.View<ChangePasswordModel>(
                Any: PartialView);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                Any: m => PartialView(m));
        }
        #endregion

        #region Change Email Address
        public ActionResult ChangeEmailAddress()
        {
            return Dispatcher.View<ChangeEmailAddressModel>(
                Any: PartialView); // was returning "View"
        }

        [HttpPost]
        public ActionResult ChangeEmailAddress(ChangeEmailAddressModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                Any: m => PartialView(m));
        }
        #endregion

        #region Set Credentials
        public ActionResult SetCredentials()
        {
            return Dispatcher.View<SetCredentialsModel>(
                Any: PartialView); // was "View"
        }

        [HttpPost]
        public ActionResult SetCredentials(SetCredentialsModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                Any: m => PartialView(m));
        }
        #endregion

        #region Manage External Credentials
        [HttpPost]
        public ActionResult LinkExternalCredentials(string Provider)
        {
            return new ChallengeResult(Provider, Url.Action("LinkExternalCredentialsCallback"), User.Identity.GetUserId());
        }

        public async Task<ActionResult> LinkExternalCredentialsCallback()
        {
            var ExternalLoginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync(ChallengeResult.XsrfKey, User.Identity.GetUserId());

            if (ExternalLoginInfo == null)
            {
                Flash.Error("Corruption detected when reading the external credentials.");
                return RedirectToAction("Settings");
            }

            return Dispatcher.Write(
                Model: new LinkExternalCredentialsCallbackModel
                    {
                        LoginProvider = ExternalLoginInfo.Login.LoginProvider,
                        ProviderKey = ExternalLoginInfo.Login.ProviderKey,
                        EmailAddress = ExternalLoginInfo.Email
                    },
                    Any: m => RedirectToAction("Settings"));
        }

        public ActionResult UnlinkExternalCredentials(UnlinkExternalCredentialsModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                Any: m => RedirectToAction("Settings"));
        }
        #endregion

        public ActionResult Confirmation(ConfirmationModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                Any: m => RedirectToAction("LocalLogin", "Authentication"));
        }
    }
}
