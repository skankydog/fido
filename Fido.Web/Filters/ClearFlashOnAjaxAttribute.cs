﻿using System.Web.Mvc;

namespace Fido.Web
{
    public class ClearFlashOnAjaxAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext FilterContext)
        {
            if (FilterContext.HttpContext.Request.IsAjaxRequest())
            {
                //FlasherWrapper.CreateFlashMessenger.Clear();
            }
        }
    }
}
