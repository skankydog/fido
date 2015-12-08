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
using Fido.DataAccess.Exceptions;

namespace Fido.Service.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        #region Change Email Address Tests
        [TestMethod]
        public void CanChangeEmailAddress()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            Guid IntitiateRegistrationConfirmationId = AuthenticationService.InitiateRegistration("santas.little.helper@skankydog.com", "28*8sdjhhjdjssd", "John", "Citizen");
            AuthenticationService.CompleteRegistration(IntitiateRegistrationConfirmationId);

            var UserService = ServiceFactory.CreateService<IUserService>();
            var UserDTO = UserService.GetByEmailAddress("santas.little.helper@skankydog.com");

            Guid ChangeEmailAddressConfirmationId = UserService.InitiateChangeEmailAddress(UserDTO.Id, "moe.szyslak@skankydog.com");
            UserService.CompleteChangeEmailAddress(ChangeEmailAddressConfirmationId);

            UserDTO = UserService.GetByEmailAddress("moe.szyslak@skankydog.com");
            Assert.AreEqual("Enabled", UserDTO.LocalCredentialState);
        }

        [TestMethod]
        [ExpectedException(typeof(EmailAddressDuplicationException))]
        public void DuplicateEmailAddressThrowsOnInitiateChangeEmailAddress()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            User UserDTO = UserService.GetByEmailAddress("bart.simpson@skankydog.com");

            UserService.InitiateChangeEmailAddress(UserDTO.Id, "homer.simpson@skankydog.com");
        }

        [TestMethod]
        [ExpectedException(typeof(UniqueFieldException))]
        public void DuplicateEmailAddressThrowsOnCompleteChangeEmailAddress()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserService = ServiceFactory.CreateService<IUserService>();

            var UserDTO = UserService.GetByEmailAddress("homer.simpson@skankydog.com");
            Guid ConfirmationId = Guid.NewGuid();

            try
            {
                // Initiate a change to an existing account
                ConfirmationId = UserService.InitiateChangeEmailAddress(UserDTO.Id, "moe.szyslak@skankydog.com");

                // Before the change is confirmed, register a new user
                AuthenticationService.InitiateRegistration("moe.szyslak@skankydog.com", "98(jsjhdJHJHSJHD00909(#(#", "Moe", "Szyslak");
            }
            catch(Exception)
            {
                // Make sure the expected exception is not coming from the initiation calls by
                // catching all exceptions and continuing...
            }

            // Attempt to complete the email address change - this should throw
        //    var Confirmations = ConfirmationService.GetAllQueuedConfirmations();
        //    var Confirmation = (from Confirmation C in Confirmations
        //                        where C.EmailAddress == "moe.szyslak@skankydog.com"
        //                        where C.Type == "Change Email Address"
        //                        select C).FirstOrDefault();
        //    ConfirmationService.MarkConfirmationAsSent(Confirmation.Id);
            UserService.CompleteChangeEmailAddress(ConfirmationId);
        }

        [TestMethod]
        [ExpectedException(typeof(EmailAddressValidationException))]
        public void InvalidEmailAddressThrowsOnChangeEmailAddress()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            User UserDTO = UserService.GetByEmailAddress("bart.simpson@skankydog.com");

            UserService.InitiateChangeEmailAddress(UserDTO.Id, "not.a.valid.email.address");
        }
        #endregion

        #region Password Tests
        [TestMethod]
        public void CanChangePassword()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserService = ServiceFactory.CreateService<IUserService>();

            var ConfirmationId = AuthenticationService.InitiateRegistration("santas.little.helper@skankydog.com", "28*8sdjhhjdjssd", "John", "Citizen");
            var UserDTO = AuthenticationService.CompleteRegistration(ConfirmationId);

            UserService.ChangeLocalPassword(UserDTO.Id, "28*8sdjhhjdjssd", "9398349DKjsdkj((#$349");

            UserDTO = AuthenticationService.LoginByLocalCredentials("santas.little.helper@skankydog.com", "9398349DKjsdkj((#$349");
            Assert.IsNotNull(UserDTO);
        }

        [TestMethod]
        [ExpectedException(typeof(PasswordValidationException))]
        public void InvalidPasswordThrowsOnChangePassword()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserService = ServiceFactory.CreateService<IUserService>();

            var ConfirmationId = AuthenticationService.InitiateRegistration("santas.little.helper@skankydog.com", "28*8sdjhhjdjssd", "John", "Citizen");
            var UserDTO = AuthenticationService.CompleteRegistration(ConfirmationId);

            UserService.ChangeLocalPassword(UserDTO.Id, "28*8sdjhhjdjssd", "weak");
        }
        #endregion

        #region Email Address Tests
        [TestMethod]
        public void CanGetUserByEmailAddress()
        {
            IUserService UserService = ServiceFactory.CreateService<IUserService>();
            Assert.IsNotNull(UserService.GetByEmailAddress("homer.simpson@skankydog.com"));
        }

        [TestMethod]
        public void EmailAddressIsIgnoredOnSave()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();

            User UserDTO = UserService.GetByEmailAddress("homer.simpson@skankydog.com");
            UserDTO.EmailAddress = "new.user@skankydog.com";
            UserService.Save(UserDTO);

            UserDTO = UserService.GetByEmailAddress("new.user@skankydog.com");
            Assert.IsNull(UserDTO);

            UserDTO = UserService.GetByEmailAddress("homer.simpson@skankydog.com");
            Assert.IsNotNull(UserDTO);
        }
        #endregion

        #region Profile Tests
        [TestMethod]
        public void CanGetProfile()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();

            var UserDTO = UserService.GetByEmailAddress("homer.simpson@skankydog.com");
            var ProfileDTO = UserService.GetProfile(UserDTO.Id);

            Assert.AreEqual("This is really just a sentence", ProfileDTO.About.Substring(0, 30));
            // Other...

        }

        [TestMethod]
        public void CanSetProfile()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();

            var UserDTO = UserService.GetByEmailAddress("homer.simpson@skankydog.com");
            var UserProfile = UserService.GetProfile(UserDTO.Id);

            UserProfile.About = "<placeholder>";
            UserProfile.Fullname.Firstname = "John";
            UserProfile.Fullname.Surname = "Citizen";
            UserService.SetProfile(UserProfile);

            var UserDto = UserService.Get(UserDTO.Id);
            Assert.AreEqual("<placeholder>", UserDto.About);
            Assert.AreEqual("John", UserDto.Fullname.Firstname);
            Assert.AreEqual("Citizen", UserDto.Fullname.Surname);
        }
        #endregion

        #region Select Tests
        //[TestMethod]
        //public void CanGetPageInSurnameOrder()
        //{
        //    var UserService = ServiceFactory.CreateService<IUserService>();
        //    var UserDTOs = UserService.GetPageInSurnameOrder();

        //    Assert.AreEqual(1, UserDTOs.Count);
        //}
        #endregion

        #region Role and Activity Tests
        [TestMethod]
        public void CanGetRolesForUser()
        {
            IUserService UserService = ServiceFactory.CreateService<IUserService>();

            User UserDTO = UserService.GetByEmailAddress("marge.simpson@skankydog.com");
            Assert.AreEqual(2, UserService.GetRoles(UserDTO.Id).Count);
        }

        [TestMethod]
        public void CanSetRolesForUser()
        {
            IUserService UserService = ServiceFactory.CreateService<IUserService>();
            Guid BartsUserId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;
            Guid MargesUserId = UserService.GetByEmailAddress("marge.simpson@skankydog.com").Id;

            IList<Role> BartsRoles = UserService.GetRoles(BartsUserId);
            IList<Role> MargesRoles = UserService.GetRoles(MargesUserId);

            Assert.AreEqual(0, BartsRoles.Count);
            Assert.AreEqual(2, MargesRoles.Count);

            UserService.SetRoles(BartsUserId, MargesRoles);

            Assert.AreEqual(2, UserService.GetRoles(BartsUserId).Count);
        }

        //[TestMethod]
        //[ExpectedException(typeof(Exception))]
        //public void SetRolesForUserThrowsOnNewRoles()
        //{
        //    IUserService UserService = ServiceFactory.CreateService<IUserService>();
        //    Guid BartsUserId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

        //    var x = UserService.SetRoles(BartsUserId, new List<Role>() {
        //                new Role { Name = "NewRole01" },
        //                new Role { Name = "NewRole02" },
        //                new Role { Name = "NewRole03" } });


        //}

        [TestMethod]
        public void CanGetActivitiesForUser()
        {
            IUserService UserService = ServiceFactory.CreateService<IUserService>();
            Guid MargesUserId = UserService.GetByEmailAddress("marge.simpson@skankydog.com").Id;

            Assert.AreEqual(2, UserService.GetRoles(MargesUserId).Count);
            Assert.AreEqual(6, UserService.GetActivities(MargesUserId).Count);
        }

        [TestMethod]
        public void CanCheckIfUserHasActivity()
        {
            IUserService UserService = ServiceFactory.CreateService<IUserService>();
            Guid HomersId = UserService.GetByEmailAddress("homer.simpson@skankydog.com").Id;
            Guid BartsId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

            Assert.IsTrue(UserService.UserHasActivity(HomersId, "Activity01"));
            Assert.IsFalse(UserService.UserHasActivity(BartsId, "Activity01"));
            Assert.IsTrue(UserService.UserHasActivity(HomersId, "Activity02"));
            Assert.IsFalse(UserService.UserHasActivity(BartsId, "Activity02"));
            Assert.IsTrue(UserService.UserHasActivity(HomersId, "Activity03"));
            Assert.IsFalse(UserService.UserHasActivity(BartsId, "Activity03"));
            Assert.IsTrue(UserService.UserHasActivity(HomersId, "Activity04"));
            Assert.IsTrue(UserService.UserHasActivity(HomersId, "Activity05"));
            Assert.IsTrue(UserService.UserHasActivity(HomersId, "Activity06"));
            Assert.IsFalse(UserService.UserHasActivity(HomersId, "Non-Existent Activity"));
            Assert.IsFalse(UserService.UserHasActivity(BartsId, "Non-Existent Activity"));
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
            Service.ServiceFactory.Boot();
        }
        #endregion
    }
}
