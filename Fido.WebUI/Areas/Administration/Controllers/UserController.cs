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

namespace Fido.WebUI.Areas.Administration.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.Read<UsersModel>(
                Id: AuthenticatedId,
                SuccessUI: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Create()
        {
            return Dispatcher.View<UserModel>(View);
        }

        [HttpPost]
        public ActionResult Create(UserModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                UI: m => RedirectToAction("Users"));
        }

        public ActionResult Read(Guid Id)       // Needed? Does the same as the Update get request - just presents
        {                                       // without the submit button. Can just check permissons from the
            return Dispatcher.Read<UserModel>(  // view to display submit button or not?
                Id: Id,
                SuccessUI: m => View(m));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.Read<UserModel>(
                Id: Id,
                SuccessUI: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(UserModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                UI: m => RedirectToAction("Users"));
        }

        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.Read<UserModel>(
                Id: Id,
                SuccessUI: m => View(m));
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(Guid Id)
        {
            return Dispatcher.Delete_<UserModel>(
                Id: Id,
                UI: () => RedirectToAction("Users"));
        }
    }
}
