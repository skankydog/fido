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
    public class AdministrationServiceTests
    {
        [TestMethod]
        public void CanDisableAndEnableLocalCredentials()
        {
            var AdminService = ServiceFactory.CreateService<IAdministrationService>();
            var UserService = ServiceFactory.CreateService<IUserService>();
            var UserDto = UserService.GetByEmailAddress("homer.simpson@skankydog.com");

            Assert.IsTrue(UserDto.LocalCredentialsAreUsable);

            AdminService.DisableLocalCredentials(UserDto.Id);
            UserDto = UserService.Get(UserDto.Id);
            Assert.IsFalse(UserDto.LocalCredentialsAreUsable);

            AdminService.EnableLocalCredentials(UserDto.Id);
            UserDto = UserService.Get(UserDto.Id);
            Assert.IsTrue(UserDto.LocalCredentialsAreUsable);
        }

        [TestMethod]
        public void CanSetEmailAddress()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CanSetLocalPassword()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CanClearLocalCredentials()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CanDisableAndEnableExternalCredentials()
        {
            var AdminService = ServiceFactory.CreateService<IAdministrationService>();
            var UserService = ServiceFactory.CreateService<IUserService>();
            var UserDto = UserService.GetByEmailAddress("homer.simpson@skankydog.com");

            Assert.IsTrue(UserDto.ExternalCredentialsAreUsable);

            AdminService.DisableExternalCredentials(UserDto.Id);
            UserDto = UserService.Get(UserDto.Id);
            Assert.IsFalse(UserDto.ExternalCredentialsAreUsable);

            AdminService.EnableExternalCredentials(UserDto.Id);
            UserDto = UserService.Get(UserDto.Id);
            Assert.IsTrue(UserDto.ExternalCredentialsAreUsable);
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
