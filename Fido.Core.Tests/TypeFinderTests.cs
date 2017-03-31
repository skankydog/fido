using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core.Bootstrapper;

namespace Fido.Core.Tests
{
    [TestClass]
    public class TypeFinderTests
    {
        [TestMethod]
        public void find_derived_classes()
        {
            TypeFinder Finder = new TypeFinder();
            List<System.Type> Types = Finder.Find<TypeToFindBase>();

            Assert.AreEqual(18, Types.Count());
        }
    }
}
