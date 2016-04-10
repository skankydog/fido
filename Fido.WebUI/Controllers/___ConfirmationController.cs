//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using Microsoft.AspNet.Identity;
//using Microsoft.Owin.Security;
//using Fido.Core;
//using Fido.Action.Models;
//using Fido.WebUI.Common;

//namespace Fido.WebUI.Controllers
//{
//    public class ConfirmationController : BaseController
//    {
//        public ActionResult Confirm(Confirmation Model)
//        {
//            return Dispatcher.SavePostedModel(
//                DataModel: Model,
//                Result: m => RedirectToAction("Login", "Authentication"));
//        }
//    }
//}
