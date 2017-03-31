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
    public class EmailAddressController : BaseController
    {
        public ActionResult Create()
        {
            return Dispatcher.Load<EmailAddress>(
                Result: m => PartialView());
        }

        [HttpPost]
        public ActionResult Create(EmailAddress Model)
        {
            return Dispatcher.Create(
                DataModel: Model,
                SuccessResult: m => ModalRedirectToLocal(Url.Action("Index", "Settings", new { Area = "Account" }, null)),
                InvalidResult: m => PartialView(m));
        }

        public ActionResult Confirm(Guid ConfirmationId)
        {
            return Dispatcher.Confirm<EmailAddress>(
                Id: ConfirmationId,
                Result: m => RedirectToAction("Create", "Login", new { Area = "Authentication" }));
        }
    }
}
