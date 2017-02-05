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
    public class UserController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.Index<Users>(
                Result: m => View());
        }

        public ActionResult IndexRead(IndexOptions IndexOptions)
        {
            return Dispatcher.Index<Users>(
                IndexOptions: IndexOptions,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Create()
        {
            return Dispatcher.CreateLoad<User>(
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Create(User Model)
        {
            return Dispatcher.Create(
                DataModel: Model,
                Result: m => RedirectToAction("Index"));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.UpdateLoad<User>(
                Id: Id,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(User Model)
        {
            return Dispatcher.Update(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "User"),
                InvalidResult: m => View(m));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.UpdateLoad<User>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(User Model)
        {
            return Dispatcher.Delete<User>(
                DataModel: Model,
                Result: m => ModalRedirectToLocal(Url.Action("Index", "User")));
        }
    }
}
