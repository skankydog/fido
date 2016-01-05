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
        public ActionResult IndexRead(IndexOptions IndexOptions)
        {
            return Dispatcher.IndexView<UsersModel>(
                IndexOptions: IndexOptions,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Index()
        {
            return Dispatcher.CreateView<UsersModel>( // Another version of IndexView?????
                Result: () => View()); // Not sure I should allow for parameterless delegates
        }

        public ActionResult Create()
        {
            return Dispatcher.CreateView(
                DataModel: new UserModel(),
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Create(UserModel Model)
        {
            return Dispatcher.Create(
                DataModel: Model,
                SuccessResult: () => RedirectToAction("Index"),
                NonsuccessResult: m => RedirectToAction("Index"));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.UpdateView<UserModel>(
                Id: Id,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(UserModel Model)
        {
            return Dispatcher.Update(
                DataModel: Model,
                SuccessResult: () => RedirectToAction("Index", "User"),
                NonsuccessResult: m => View(m));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.DeleteView<UserModel>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(UserModel Model)
        {
            return Dispatcher.Delete_<UserModel>(
                DataModel: Model,
                Result: () => ModalRedirectToLocal(Url.Action("Index", "User")));
        }
    }
}
