﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Service.Implementation;
using Fido.Service.Exceptions;

namespace Fido.Service.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class RoleServiceTests
    {
        [TestMethod]
        public void get_users_in_role()
        {
            var RoleService = ServiceFactory.CreateService<IRoleService>();

            var RoleDTO = RoleService.GetByName("All Activities");
            var Users = RoleService.GetUsersInRole(RoleDTO.Id);

            Assert.AreEqual(1, Users.Count);
        }

        #region Activity Tests
        [TestMethod]
        public void get_activities_for_role()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();

            Role RoleDTO = RoleService.GetByName("Role 2");
            Assert.AreEqual(4, RoleService.GetActivitiesForRole(RoleDTO.Id).Count);
        }

        [TestMethod]
        public void set_activities_for_role()
        {
            var RoleService = ServiceFactory.CreateService<IRoleService>();
            var FirstRoleId = RoleService.GetByName("All Activities").Id;
            var SecondRoleId = RoleService.GetByName("Role 3").Id;

            var FirstRoleActivities = RoleService.GetActivitiesForRole(FirstRoleId);
            var SecondRoleActivities = RoleService.GetActivitiesForRole(SecondRoleId);

            Assert.AreEqual(6, FirstRoleActivities.Count);
            Assert.AreEqual(4, SecondRoleActivities.Count);

            RoleService.SetActivitiesForRole(FirstRoleId, SecondRoleActivities);

            Assert.AreEqual(4, RoleService.GetActivitiesForRole(FirstRoleId).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void set_activities_for_role_throws_on_new_role()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();
            Guid RoleId = RoleService.GetByName("All Activities").Id;

            RoleService.SetActivitiesForRole(RoleId, new List<Activity>() {
                        new Activity { Name = "NewActivity01", Area = "Area", ReadWrite = "Action" },
                        new Activity { Name = "NewActivity02", Area = "Area", ReadWrite = "Action" },
                        new Activity { Name = "NewActivity03", Area = "Area", ReadWrite = "Action" } });
        }
        #endregion

        #region Name Tests
        [TestMethod]
        public void get_role_by_name()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();
            Assert.IsNotNull(RoleService.GetByName("All Activities"));
        }

        [TestMethod]
        public void role_name_free()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();

            Assert.IsFalse(RoleService.NameFree("All Activities"));
            Assert.IsTrue(RoleService.NameFree("Non-Existant Role"));
        }

        [TestMethod]
        [ExpectedException(typeof(Fido.DataAccess.Exceptions.UniqueFieldException))]
        public void insert_role_throws_on_duplicate_name()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();

            Role RoleDTO = new Role
            {
                Name = "All Activities"
            };

            RoleService.Save(RoleDTO);
        }

        [TestMethod]
        [ExpectedException(typeof(Fido.DataAccess.Exceptions.UniqueFieldException))]
        public void update_role_throws_on_duplicate_name()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();

            Role RoleDTO = RoleService.GetByName("Role 2");
            RoleDTO.Name = "Role 3";

            RoleService.Save(RoleDTO);
        }
        #endregion

        #region Initialisation
        [TestInitialize]
        public void TestInitialise()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();
        }

        [ClassInitialize]
        public static void Initialise(TestContext Context)
        {
            Service.ServiceFactory.Boot();
        }
        #endregion
    }
}
