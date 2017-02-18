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
    public class ActivityController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.Index<ActivityIndex>(
                Result: m => View());
        }

        public ActionResult List(ListOptions ListOptions)
        {
            return Dispatcher.List<ActivityList>(
                IndexOptions: ListOptions,
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
            return Dispatcher.Update(
                DataModel: Model,
                SuccessResult: m => RedirectToAction("Index", "Activity"),
                InvalidResult: m => View(m));
        }
    }
}
