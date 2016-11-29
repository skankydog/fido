﻿using System;
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
        public void CanGetActivity()
        {
            IActivityService ActivityService = ServiceFactory.CreateService<IActivityService>();

            Assert.IsNull(ActivityService.Get("None", "None", "None"));
            Assert.IsNotNull(ActivityService.Get("Action 1", "Controller/Model 1", "Namespace 1"));
        }

        [TestMethod]
        public void CanGetPageOfActivities()
        {
            IActivityService ActivityService = ServiceFactory.CreateService<IActivityService>();

            Assert.AreEqual(3, ActivityService.GetPageInDefaultOrder('a', 0, 10, "Namespace 1").Count());
            
            Assert.AreEqual("Controller/Model 6", ActivityService.GetPageInDefaultOrder('d', 0, 10, "Namespace").FirstOrDefault().Name);
            Assert.AreEqual("Controller/Model 1", ActivityService.GetPageInDefaultOrder('a', 0, 10, "Namespace").FirstOrDefault().Name);
            
            Assert.AreEqual("Namespace 4", ActivityService.GetPageInAreaOrder('d', 0, 10, "Namespace").FirstOrDefault().Area);
            Assert.AreEqual("Namespace 1", ActivityService.GetPageInAreaOrder('a', 0, 10, "Namespace").FirstOrDefault().Area);
            
            Assert.AreEqual("Controller/Model 6", ActivityService.GetPageInActionOrder('a', 0, 10, "Namespace").FirstOrDefault().Name);
            Assert.AreEqual("Controller/Model 2", ActivityService.GetPageInActionOrder('d', 0, 10, "Namespace").FirstOrDefault().Name);
            
            Assert.AreEqual(0, ActivityService.GetPageInDefaultOrder('a', 0, 100, "does not exist").Count());

            Assert.AreEqual("Controller/Model 1", ActivityService.GetPageInDefaultOrder('a', 0, 1, "Namespace").FirstOrDefault().Name);
            Assert.AreEqual("Controller/Model 2", ActivityService.GetPageInDefaultOrder('a', 1, 1, "Namespace").FirstOrDefault().Name);
            Assert.AreEqual("Controller/Model 3", ActivityService.GetPageInDefaultOrder('a', 2, 1, "Namespace").FirstOrDefault().Name);
            Assert.AreEqual("Controller/Model 4", ActivityService.GetPageInDefaultOrder('a', 3, 1, "Namespace").FirstOrDefault().Name);
        }

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