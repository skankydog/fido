using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.ViewModel.Models.Account;
using Fido.Web.Common;

namespace Fido.Web.Areas.Account.Controllers
{
    public class PasswordController : BaseController
    {
        public ActionResult Update()
        {
            return Dispatcher.Load<Password>(
                Result: m => PartialView());
        }

        [HttpPost]
        public ActionResult Update(Password Model)
        {
            return Dispatcher.Update(
                DataModel: Model,
                SuccessResult: m => ModalRedirectToLocal(Url.Action("Index", "Settings")),
                InvalidResult: m => PartialView(m));
        }
    }
}
