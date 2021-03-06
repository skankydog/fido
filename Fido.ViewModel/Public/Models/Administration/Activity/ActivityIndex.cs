﻿// http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.ViewModel.Implementation;

namespace Fido.ViewModel.Models.Administration
{
    public class ActivityIndex : Model<ActivityIndex>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        #endregion

        public ActivityIndex()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.NA)
        { }
    }
}
