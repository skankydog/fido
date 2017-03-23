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
    public class LoginController : BaseController
    {
        public ActionResult Create(string ReturnUrl = @"/Home/Index")
        {
            ViewBag.ReturnUrl = ReturnUrl;

            // As this is the login, we just need to display the view.
            return View();
        }

        [HttpPost]
        public ActionResult Create(Login Model, string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;

            return Dispatcher.Create(
                DataModel: Model,
                SuccessResult: m => RedirectToLocal(ReturnUrl),
                InvalidResult: m => View(m));
        }

        [HttpPost]
        [Authorize] // TO DO: Use the framework for this too
        public ActionResult Delete()
        {
            // As this is the logout, there is no VM. The generation of
            // the view does not need service layer functions.
            SignOut();
            LoggedInCredentialState = "None";
            Flash.Success("You have successfully logged out.");

            return RedirectToAction("Create", "Login");
        }
    }
}
