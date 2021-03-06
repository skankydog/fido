﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.ViewModel.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.ViewModel.Models.Administration
{
    public class Activity : Model<Activity>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public IList<Role> AllRoles = new List<Role>();

        public string Area { get; set; }
        public string Name { get; set; }
        public string ReadWrite { get; set; }
        public string FullQualification { get; set; }

        public IList<Guid> SelectedRoles { get; set; }
        #endregion

        public Activity()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.NA)
        { }

        public override Activity Prepare(Activity Model)
        {
            var RoleService = ServiceFactory.CreateService<IRoleService>();
            Model.AllRoles = Mapper.Map<IList<Dtos.Role>, IList<Role>>(RoleService.GetAll().OrderBy(r => r.Name).ToList());

            return Model;
        }

        public override Activity Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                var ActivityService = ServiceFactory.CreateService<IActivityService>();

                var Activity = ActivityService.Get(Id);
                var Model = Mapper.Map<Dtos.Activity, Activity>(Activity);

                return Model;
            }
        }
    }
}
