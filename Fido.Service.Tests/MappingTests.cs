using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using Fido.Core;

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

        [ClassInitialize]
        public static void Initialise(TestContext Context)
        {
            ServiceFactory.Boot();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
        }
    }
}
