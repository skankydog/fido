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

namespace Fido.WebUI.Areas.Account.Controllers
{
    public class ExternalCredentialsController : BaseController
    {
        #region Link
        [HttpPost]
        public ActionResult Link(string Provider)
        {
            return new ChallengeResult(Provider, Url.Action("Link"), User.Identity.GetUserId());
        }

        public async Task<ActionResult> Link()
        {
            var ExternalLoginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync(ChallengeResult.XsrfKey, User.Identity.GetUserId());

            if (ExternalLoginInfo == null)
            {
                Flash.Error("Corruption detected when reading external credentials.");
                return RedirectToAction("Index", "Settings");
            }

            return Dispatcher.SavePostedModel(
                DataModel: new ExternalCredentialVM
                    {
                        Id = Guid.Empty,
                        LoginProvider = ExternalLoginInfo.Login.LoginProvider,
                        ProviderKey = ExternalLoginInfo.Login.ProviderKey,
                        EmailAddress = ExternalLoginInfo.Email
                    },
                    SuccessResult: m => RedirectToAction("Index", "Settings"),
                    NonsuccessResult: m => RedirectToAction("Index", "Settings"));
        }
        #endregion

        #region Unlink
        public ActionResult Unlink(ExternalCredentialVM Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Settings"),
                NonsuccessResult: m => RedirectToAction("Index", "Settings"));
        }
        #endregion
    }
}
