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
    public class ActivityServiceTests
    {
        [TestMethod]
        public void CanGetActivityByName()
        {
            IActivityService ActivityService = ServiceFactory.CreateService<IActivityService>();
            Assert.IsNotNull(ActivityService.GetByName("Activity01"));
        }

        #region Name Duplication Tests
        [TestMethod]
        public void CanCheckActivityNameIsFree()
        {
            IActivityService ActivityService = ServiceFactory.CreateService<IActivityService>();

            Assert.IsFalse(ActivityService.NameFree("Activity01"));
            Assert.IsTrue(ActivityService.NameFree("Non-Existant Activity"));
        }

        //[TestMethod]
        //[ExpectedException(typeof(Fido.DataAccess.Exceptions.UniqueFieldException))]
        //public void DuplicateNameDetectedOnActivityInsert()
        //{
        //    IActivityService ActivityService = ServiceFactory.CreateService<IActivityService>();

        //    Activity ActivityDTO = new Activity
        //    {
        //        Name = "Activity01",
        //        Area ="1",
        //        Action = "2"
        //    };

        //    ActivityService.Save(ActivityDTO);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(Fido.DataAccess.Exceptions.UniqueFieldException))]
        //public void DuplicateNameDetectedOnActivityUpdate()
        //{
        //    IActivityService ActivityService = ServiceFactory.CreateService<IActivityService>();

        //    Activity ActivityDTO = ActivityService.GetByName("Activity01");
        //    ActivityDTO.Name = "Activity02";

        //    ActivityService.Save(ActivityDTO);
        //}
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
