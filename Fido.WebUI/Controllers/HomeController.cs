using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Fido.Core;
using Fido.Action;
using Fido.Action.Models.Common;
using Fido.WebUI.Extensions;
using Fido.WebUI.Flash.Messages;
using Fido.WebUI.Common;

namespace Fido.WebUI.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return Dispatcher.Load<Home>(
                Result: m => View(m));
        }

        public ActionResult About()
        {
            return PartialView();
        }

        public ActionResult Contact()
        {
            return Dispatcher.Load<Contact>(
                Result: m => View(m));
        }

        public ActionResult ModalRedirectToLocal(ModalRedirectToLocalModel Model)
        {
            using (new FunctionLogger(Log))
            {
                if (!Url.IsLocalUrl(Model.Location))
                {
                    Log.WarnFormat("Attempt to redirect to non-local location: {0}", Model.Location);
                    Model.Location = Url.Action("Index", "Home");
                }

                Log.InfoFormat("RedirectorModel.Location={0}", Model.Location);
                return View(Model);
            }
        }
    }
}
