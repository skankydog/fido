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
    public class ForgottenPasswordController : BaseController
    {
        public ActionResult Initiate()
        {
            return Dispatcher.Create(
                new ForgottenPasswordInitiate(),
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Initiate(ForgottenPasswordInitiate Model)
        {
            return Dispatcher.Save(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Login"),
                InvalidResult: m => View(m));
        }

        public ActionResult Complete(Guid ConfirmationId)
        {
            return Dispatcher.Load<ForgottenPasswordComplete>(
                Id: ConfirmationId,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Complete(ForgottenPasswordComplete Model)
        {
            return Dispatcher.Save(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Home"),
                InvalidResult: m => View(m));
        }
    }
}
