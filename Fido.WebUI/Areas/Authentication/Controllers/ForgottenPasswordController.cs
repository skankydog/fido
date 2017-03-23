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
    public class ForgottenPasswordController : BaseController
    {
        public ActionResult Create()
        {
            return Dispatcher.Load<ForgottenPassword>(
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Create(ForgottenPassword Model)
        {
            return Dispatcher.Create(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Create", "Login"),
                InvalidResult: m => View(m));
        }
    }
}
