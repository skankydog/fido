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

        public ActionResult View(Guid Id)
        {
            return Dispatcher.Load<Activity>(
                Id: Id,
                Result: m => View(m));
        }
    }
}
