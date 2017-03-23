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
    public class ResetPasswordController : BaseController
    {
        public ActionResult Update(Guid ConfirmationId)
        {
            return Dispatcher.Load<ResetPassword>(
                Id: ConfirmationId,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(ResetPassword Model)
        {
            return Dispatcher.Update(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Home"),
                InvalidResult: m => View(m));
        }
    }
}
