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

//namespace Fido.WebUI.Areas.Authentication.Controllers
//{
//    [RequireHttps]
//    [AllowAnonymous]
//    public class RegistrationCompleteController : BaseController
//    {
//        public ActionResult Confirm(Guid ConfirmationId)
//        {
//            return Dispatcher.Confirm<RegistrationComplete>(
//                Id: ConfirmationId,
//                Result: m => View(m));
//        }

//        //[HttpPost]
//        //public ActionResult Update(RegistrationComplete Model)
//        //{
//        //    return Dispatcher.Save(
//        //        DataModel: Model,
//        //        SuccessResult: m => RedirectToAction("Index", "Home"),
//        //        InvalidResult: m => View(m));
//        //}
//    }
//}
