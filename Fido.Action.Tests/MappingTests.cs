using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using Fido.Core;
using Fido.Action;

namespace Fido.Service.Tests
{
    [TestClass]
    public class MappingTests
    {
        [TestMethod]
        public void Mapping()
        {
            Mapper.AssertConfigurationIsValid();
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
            ActionFactory.Boot();
        }
        #endregion
    }
}
