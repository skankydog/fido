// http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models.Administration
{
    public class Home : Model<Home>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        #endregion

        public Home()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
        { }

        public override Home Read(IndexOptions IndexOptions)
        {
            return null;
        }
    }
}
