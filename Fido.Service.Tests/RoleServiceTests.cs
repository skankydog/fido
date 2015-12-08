using System;
using System.Collections.Generic;
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
    [TestClass]
    public class RoleServiceTests
    {
        [TestMethod]
        public void CanGetRoleByName()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();
            Assert.IsNotNull(RoleService.GetByName("AllActivitiesRole"));
        }

        [TestMethod]
        public void CanGetUsersInRole()
        {
            var RoleService = ServiceFactory.CreateService<IRoleService>();

            var RoleDTO = RoleService.GetByName("AllActivitiesRole");
            var Users = RoleService.GetUsersInRole(RoleDTO.Id);

            Assert.AreEqual(1, Users.Count);
        }

        [TestMethod]
        public void CanGetActivitiesForRole()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();

            Role RoleDTO = RoleService.GetByName("Role02");
            Assert.AreEqual(4, RoleService.GetActivitiesForRole(RoleDTO.Id).Count);
        }

        [TestMethod]
        public void CanSetActivitiesForRole()
        {
            var RoleService = ServiceFactory.CreateService<IRoleService>();
            var FirstRoleId = RoleService.GetByName("AllActivitiesRole").Id;
            var SecondRoleId = RoleService.GetByName("Role03").Id;

            var FirstRoleActivities = RoleService.GetActivitiesForRole(FirstRoleId);
            var SecondRoleActivities = RoleService.GetActivitiesForRole(SecondRoleId);

            Assert.AreEqual(6, FirstRoleActivities.Count);
            Assert.AreEqual(4, SecondRoleActivities.Count);

            RoleService.SetActivitiesForRole(FirstRoleId, SecondRoleActivities);

            Assert.AreEqual(4, RoleService.GetActivitiesForRole(FirstRoleId).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetActivitiesForRoleThrowsOnNewRoles()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();
            Guid RoleId = RoleService.GetByName("AllActivitiesRole").Id;

            RoleService.SetActivitiesForRole(RoleId, new List<Activity>() {
                        new Activity { Name = "NewActivity01" },
                        new Activity { Name = "NewActivity02" },
                        new Activity { Name = "NewActivity03" } });
        }

        #region Name Duplication Tests
        [TestMethod]
        public void CanCheckRoleNameIsFree()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();

            Assert.IsFalse(RoleService.NameFree("AllActivitiesRole"));
            Assert.IsTrue(RoleService.NameFree("Non-Existant Role"));
        }

        [TestMethod]
        [ExpectedException(typeof(Fido.DataAccess.Exceptions.UniqueFieldException))]
        public void DuplicateNameDetectedOnRoleInsert()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();

            Role RoleDTO = new Role
            {
                Name = "AllActivitiesRole"
            };

            RoleService.Save(RoleDTO);
        }

        [TestMethod]
        [ExpectedException(typeof(Fido.DataAccess.Exceptions.UniqueFieldException))]
        public void DuplicateNameDetectedOnRoleUpdate()
        {
            IRoleService RoleService = ServiceFactory.CreateService<IRoleService>();

            Role RoleDTO = RoleService.GetByName("Role02");
            RoleDTO.Name = "Role03";

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
            //DataAccess.DataAccessFactory.CreateBootstrapperEngine().Bootstrap();
            Service.ServiceFactory.Boot();
        }
        #endregion
    }
}
