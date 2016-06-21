using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action.Models.Account;
using Fido.WebUI.Common;

namespace Fido.WebUI.Areas.Account.Controllers
{
    public class EmailAddressController : BaseController
    {
        public ActionResult Initiate()
        {
            return Dispatcher.Create(
                new ChangeEmailAddressInitiate(),
                Result: m => PartialView());
        }

        [HttpPost]
        public ActionResult Initiate(ChangeEmailAddressInitiate Model)
        {
            return Dispatcher.Save(
                DataModel: Model,
                SuccessResult: m => ModalRedirectToLocal(Url.Action("Index", "Settings", new { Area = "Account" }, null)),
                InvalidResult: m => PartialView(m));
        }
    }
}
