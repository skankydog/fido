using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.ViewModel;
using Fido.ViewModel.Models.Administration;
using Fido.Web.Common;
using Fido.Web.Binders;

namespace Fido.Web.Areas.Administration.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.Index<UserIndex>(
                Result: m => View());
        }

        public ActionResult List(ListOptions ListOptions)
        {
            return Dispatcher.List<UserList>(
                IndexOptions: ListOptions,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Create()
        {
            return Dispatcher.Load<UserCreate>(
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Create(UserCreate Model)
        {
            return Dispatcher.Create(
                DataModel: Model,
                Result: m => RedirectToAction("Index"));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.Load<User>(
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
            return Dispatcher.Load<User>(
                Id: Id,
                Result: m => PartialView(m));
        }

        [HttpPost]
        public ActionResult Delete(User Model)
        {
            return Dispatcher.DeleteIt<User>(
                DataModel: Model,
                Result: m => ModalRedirectToLocal(Url.Action("Index", "User")));
        }
    }
}
