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
        //[ModelBinder(typeof(IndexParamsModelBinder))]
        public ActionResult IndexRead(IndexOptions Params)
        {
            return Dispatcher.Read<UsersModel>(
                Id: AuthenticatedId,
                IndexOptions: Params,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Index()
        {
            return Dispatcher.View<UsersModel>(View);
        }

        public ActionResult Create()
        {
            return Dispatcher.View<UserModel>(View);
        }

        [HttpPost]
        public ActionResult Create(UserModel Model)
        {
            return Dispatcher.Write(
                DataModel: Model,
                AnyResult: m => RedirectToAction("Index"));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.Read<UserModel>(
                Id: Id,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(UserModel Model)
        {
            return Dispatcher.Write(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "User"),
                InvalidResult: m => View(m),
                FailureResult: m => View(m));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.Read<UserModel>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(UserModel Model)
        {
            return Dispatcher.Delete_<UserModel>(
                DataModel: Model,
                Result: () => PartialView()); // Modal - redirect happens from client side... thinking, thinking, thinking.
        }
    }
}
