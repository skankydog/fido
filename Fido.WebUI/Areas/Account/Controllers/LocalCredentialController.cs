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
    public class LocalCredentialController : BaseController
    {
        public ActionResult Initiate()
        {
            return Dispatcher.ReturnEmptyModel( // TO DO: Make this a read for consistency!!
                new SetLocalCredentialInitiate(),
                Result: m => PartialView());
        }

        [HttpPost]
        public ActionResult Initiate(SetLocalCredentialInitiate Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => ModalRedirectToLocal(Url.Action("Index", "Settings", new { Area = "Account" }, null)),
                InvalidResult: m => PartialView(m));
        }
    }
}
