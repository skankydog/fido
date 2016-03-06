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
    public class EmailAddressController : BaseController
    {
        public ActionResult Update()
        {
            return Dispatcher.ReturnEmptyModel(
                new EmailAddressVM(),
                Result: m => PartialView());
        }

        [HttpPost]
        public ActionResult Update(EmailAddressVM Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => ModalRedirectToLocal(Url.Action("Index", "Settings", new { Area = "Account" }, null)),
                NonsuccessResult: m => PartialView(m));
        }
    }
}
