// http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Action.Implementation;

namespace Fido.Action.Models.Common
{
    public class Contact : Model<Contact>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        #endregion

        public Contact()
            : base(ReadAccess: Access.Anonymous, WriteAccess: Access.Anonymous)
        { }

        public override Contact Read(Guid Id)
        {
            return new Contact();
        }
    }
}
