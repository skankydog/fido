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
    public class PasswordController : BaseController
    {
        public ActionResult Update()
        {
            return Dispatcher.ReturnEmptyModel( // TO DO: No read, however!!   Or should I??
                new Password(),
                Result: m => PartialView());
        }

        [HttpPost]
        public ActionResult Update(Password Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => ModalRedirectToLocal(Url.Action("Index", "Settings", new { Area = "Account" }, null)),
                InvalidResult: m => PartialView(m));
        }
    }
}
