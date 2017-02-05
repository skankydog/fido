using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action;
using Fido.Action.Models.Administration;
using Fido.WebUI.Common;
using Fido.WebUI.Binders;

namespace Fido.WebUI.Areas.Administration.Controllers
{
    public class ConfirmationController : BaseController
    {
        public ActionResult Index(Guid Id)
        {
            return Dispatcher.Index<Confirmations>(
                Id: Id,
                Result: m => View(m));
        }

        public ActionResult IndexRead(IndexOptions IndexOptions)
        {
            return Dispatcher.Index<Confirmations>(
                IndexOptions: IndexOptions,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.UpdateLoad<Confirmation>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(Confirmation Model)
        {
            return Dispatcher.Delete<Confirmation>(
                DataModel: Model,
                Result: m => ModalRedirectToLocal(Url.Action("Index", "Confirmation", new { Id = Model.UserId })));
        }
    }
}
