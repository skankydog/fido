using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action;
using Fido.Action.Models;
using Fido.WebUI.Common;
using Fido.WebUI.Binders;

namespace Fido.WebUI.Controllers
{
    public class VirtualController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.ReturnIndexWrapper(
                DataModel: new Virtuals(),
                Result: m => View());
        }

        public ActionResult IndexRead(IndexOptions IndexOptions)
        {
            return Dispatcher.ReturnLoadedModel<Virtuals>(
                IndexOptions: IndexOptions,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Create()
        {
            return Dispatcher.ReturnEmptyModel(
                DataModel: new Virtual(),
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Create(Virtual Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                Result: m => RedirectToAction("Index"));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.ReturnLoadedModel<Virtual>(
                Id: Id,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(Virtual Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "User"),
                InvalidResult: m => View(m));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.ReturnLoadedModel<Virtual>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(Virtual Model)
        {
            return Dispatcher.DeletePostedModel<Virtual>(
                DataModel: Model,
                Result: m => ModalRedirectToLocal(Url.Action("Index", "User")));
        }
    }
}
