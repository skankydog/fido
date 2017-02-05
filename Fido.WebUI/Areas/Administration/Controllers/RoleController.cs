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
    public class RoleController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.Index<Roles>(
                Result: m => View());
        }

        public ActionResult IndexRead(IndexOptions IndexOptions)
        {
            return Dispatcher.Index<Roles>(
                IndexOptions: IndexOptions,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Create()
        {
            return Dispatcher.CreateLoad<Role>(
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Create(Role Model)
        {
            return Dispatcher.Create(
                DataModel: Model,
                Result: m => RedirectToAction("Index"));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.UpdateLoad<Role>(
                Id: Id,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(Role Model)
        {
            return Dispatcher.Update(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Role"),
                InvalidResult: m => View(m));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.UpdateLoad<Role>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(Role Model)
        {
            return Dispatcher.Delete<Role>(
                DataModel: Model,
                Result: m => ModalRedirectToLocal(Url.Action("Index", "Role")));
        }
    }
}
