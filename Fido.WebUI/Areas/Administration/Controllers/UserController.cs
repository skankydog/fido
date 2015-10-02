﻿using System;
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
        public ActionResult IndexRead(IndexParams Params)
        {
            return Dispatcher.Read<UsersModel>(
                Id: AuthenticatedId,
                Params: Params,
                Success: m => Json(m, JsonRequestBehavior.AllowGet));
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
                Model: Model,
                Any: m => RedirectToAction("Index"));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.Read<UserModel>(
                Id: Id,
                Success: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(UserModel Model)
        {
            return Dispatcher.Write(
                Model: Model,
                Success: m => RedirectToAction("Index", "User"),
                Invalid: m => View(m),
                Failure: m => View(m));
        }

        public ActionResult DeleteConfirmation(Guid Id)
        {
            return Dispatcher.Read<UserModel>(
                Id: Id,
                Success: m => View(m));
        }

        [HttpPost]
        public ActionResult Delete(Guid Id)
        {
            return Dispatcher.Delete_<UserModel>(
                Id: Id,
                Any: () => RedirectToAction("Users"));
        }
    }
}
