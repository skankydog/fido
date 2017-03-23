using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.ViewModel.Models.Authentication;
using Fido.WebUI.Common;

namespace Fido.WebUI.Areas.Authentication.Controllers
{
    [RequireHttps]
    [AllowAnonymous]
    public class RegistrationController : BaseController
    {
        public ActionResult Create()
        {
            return Dispatcher.Load<Registration>(
                Result: m => View());
        }

        [HttpPost]
        public ActionResult Create(Registration Model)
        {
            return Dispatcher.Create(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Home"),
                InvalidResult: m => View(m));
        }

        public ActionResult Confirm(Guid ConfirmationId)
        {
            return Dispatcher.Confirm<Registration>(
                Id: ConfirmationId,
                Result: m => RedirectToAction("Create", "Login"));
        }
    }
}
