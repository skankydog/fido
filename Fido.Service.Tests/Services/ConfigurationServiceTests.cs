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
    public class ConfigurationServiceTests
    {
        [TestMethod]
        public void CanSetConfiguration()
        {
            var ConfigurationService = ServiceFactory.CreateService<IConfigurationService>();
            
            var OriginalConfiguration = ConfigurationService.Get();
            OriginalConfiguration.PasswordChangePolicyDays = 111;
            ConfigurationService.Set(OriginalConfiguration);

            var ChangedConfiguration = ConfigurationService.Get();
            Assert.AreEqual(111, ChangedConfiguration.PasswordChangePolicyDays);
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
