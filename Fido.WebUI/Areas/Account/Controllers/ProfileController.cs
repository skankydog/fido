using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Fido.Core;
using Fido.Action.Models.Account;
using Fido.WebUI.Common;

namespace Fido.WebUI.Areas.Account.Controllers
{
    public class ProfileController : BaseController
    {
        public ActionResult Update()
        {
            return Dispatcher.Load<Profile>(
                Id: AuthenticatedId,
                Result: m => View(m));
        }

        [HttpPost]
        public ActionResult Update(Profile Model)
        {
            return Dispatcher.Save(
                DataModel: Model,
                Result: m => View(m));
        }

        // I used to have this function in a UserImageController and use it to return the image to the
        // browser. This isn't a bad thing - caching etc would be honered, but it did seperate the
        // profile model - keeping this code in case I want to implement later as a seperate way of
        // returning images...
        // public ActionResult Image(Guid Id)
        // {
        //    return Dispatcher.ReturnLoadedModel<ProfileImageVM>(
        //        Id: Id,
        //        Result:
        //            m => m != null && m.Image != null
        //                ? new FileContentResult(m.Image, "image/jpeg")
        //                : null);
        // }
    }
}
