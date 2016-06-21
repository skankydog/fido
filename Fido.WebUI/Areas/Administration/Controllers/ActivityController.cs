﻿using System;
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
    public class ActivityController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.View(
                DataModel: new Activities(),
                Result: m => View());
        }

        public ActionResult IndexRead(IndexOptions IndexOptions)
        {
            return Dispatcher.Load<Activities>(
                IndexOptions: IndexOptions,
                Result: m => Json(m, JsonRequestBehavior.AllowGet));
        }

        public ActionResult Update(Guid Id)
        {
            return Dispatcher.Load<Activity>(
                Id: Id,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(Activity Model)
        {
            return Dispatcher.Save(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Activity"),
                InvalidResult: m => View(m));
        }
    }
}
