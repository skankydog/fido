//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Fido.Core.Bootstrapper;

//namespace Fido.Core.Tests
//{
//    [TestClass]
//    public class PasswordGeneratorTests
//    {
//        [TestMethod]
//        public void generated_passwords_are_valid()
//        {
//            Assert.IsTrue(PasswordAdvisor.CheckStrength(PasswordGenerator.GenerateStrongPassword()) >= PasswordScore.Strong);
//            Assert.IsTrue(PasswordAdvisor.CheckStrength(PasswordGenerator.GenerateStrongPassword()) >= PasswordScore.Strong);
//            Assert.IsTrue(PasswordAdvisor.CheckStrength(PasswordGenerator.GenerateStrongPassword()) >= PasswordScore.Strong);
//            Assert.IsTrue(PasswordAdvisor.CheckStrength(PasswordGenerator.GenerateStrongPassword()) >= PasswordScore.Strong);
//            Assert.IsTrue(PasswordAdvisor.CheckStrength(PasswordGenerator.GenerateStrongPassword()) >= PasswordScore.Strong);
//        }
//    }
//}
