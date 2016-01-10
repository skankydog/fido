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

namespace Fido.WebUI.Areas.Administration.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.ReturnIndexWrapper(
                DataModel: new UsersModel(),
                Result: m => View());
        }

        public ActionResult IndexRead(IndexOptions IndexOptions)
        {
            return Dispatcher.ReturnLoadedModel<UsersModel>(
                IndexOptions: IndexOptions,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Create()
        {
            return Dispatcher.ReturnEmptyModel(
                DataModel: new UserModel(),
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Create(UserModel Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index"),
                NonsuccessResult: m => RedirectToAction("Index"));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.ReturnLoadedModel<UserModel>(
                Id: Id,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(UserModel Model)
        {
            return Dispatcher.SavePostedModel(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "User"),
                NonsuccessResult: m => View(m));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.ReturnLoadedModel<UserModel>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(UserModel Model)
        {
            return Dispatcher.DeletePostedModel<UserModel>(
                DataModel: Model,
                Result: m => ModalRedirectToLocal(Url.Action("Index", "User")));
        }
    }
}
