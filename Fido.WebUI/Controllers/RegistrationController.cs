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
    public class RegistrationController : BaseController
    {
        public ActionResult Initiate()
        {
            return Dispatcher.ReturnEmptyModel(
                new RegistrationInitiate(),
                Result: m => View());
        }

        [HttpPost]
        public ActionResult Initiate(RegistrationInitiate Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Home"),
                InvalidResult: m => View(m));
        }

        public ActionResult Complete(Guid ConfirmationId)
        {
            return Dispatcher.ReturnLoadedModel<RegistrationComplete>(
                Id: ConfirmationId,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Complete(RegistrationComplete Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Home"),
                InvalidResult: m => View(m));
        }
    }
}
