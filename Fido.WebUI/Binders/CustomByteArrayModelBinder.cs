using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fido.WebUI.Binders
{
    public class CustomByteArrayModelBinder : ByteArrayModelBinder
    {
        public override object BindModel(ControllerContext CntrContext, ModelBindingContext BndgContext)
        {
            var File = CntrContext.HttpContext.Request.Files[BndgContext.ModelName];

            if (File != null)
            {
                if (File.ContentLength > 0)
                {
                    var FileBytes = new byte[File.ContentLength];
                    File.InputStream.Read(FileBytes, 0, FileBytes.Length);

                    return FileBytes;
                }

                return null;
            }

            return base.BindModel(CntrContext, BndgContext);
        }
    }
}
