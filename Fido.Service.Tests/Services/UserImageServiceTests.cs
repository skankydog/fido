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
    public class UserImageServiceTests
    {
        [TestMethod]
        public void CanXXXX()
        {
            IActivityService ActivityService = ServiceFactory.CreateService<IActivityService>();
            Assert.IsNotNull(ActivityService.GetByName("Controller/Model 1"));
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
