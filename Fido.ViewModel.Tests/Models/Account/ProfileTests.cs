using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.ViewModel;
using Fido.ViewModel.Implementation;
using Fido.ViewModel.Models;
using Fido.ViewModel.Models.Account;
using Fido.ViewModel.Models.Administration;
using Fido.ViewModel.Models.Authentication;
using Fido.ViewModel.Tests.Mocks;

namespace Fido.ViewModel.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ProfileTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;

        [TestMethod]
        public void profile_read()
        {
            const string FIRSTNAME = "read test firstname";
            const string SURNAME = "read test surname";
            const string ABOUT = "read test about text";
            const int IMAGESIZE = 100;
            var UpdatedProfile = UpdateProfile(FIRSTNAME, SURNAME, ABOUT, IMAGESIZE);

            var ReadProfile = MockDispatcher.Load<Profile>(
                Id: UpdatedProfile.Id,
                Result: m => m) as Profile;

            Assert.IsNotNull(ReadProfile);
            Assert.AreEqual(FIRSTNAME, ReadProfile.Firstname);
            Assert.AreEqual(SURNAME, ReadProfile.Surname);
            Assert.AreEqual(ABOUT, ReadProfile.About);
            Assert.AreEqual(IMAGESIZE, ReadProfile.Image.Count());
            Assert.IsFalse(ReadProfile.IsNew);
            Assert.AreEqual(UpdatedProfile.Id, ReadProfile.Id);
        }

        [TestMethod]
        public void profile_update()
        {
            const string FIRSTNAME = "update test firstname";
            const string SURNAME = "update test surname";
            const string ABOUT = "update test about text";
            const int IMAGESIZE = 200;

            var ReadProfile = GetProfile();

            ReadProfile.Firstname = FIRSTNAME;
            ReadProfile.Surname = SURNAME;
            ReadProfile.About = ABOUT;
            ReadProfile.Image = new byte[IMAGESIZE];
            MockDispatcher.Update(
                DataModel: ReadProfile,
                Result: m => m);

            var UpdatedProfile = GetProfile();

            Assert.IsNotNull(UpdatedProfile);
            Assert.AreEqual(FIRSTNAME, UpdatedProfile.Firstname);
            Assert.AreEqual(SURNAME, UpdatedProfile.Surname);
            Assert.AreEqual(ABOUT, UpdatedProfile.About);
            Assert.AreEqual(IMAGESIZE, UpdatedProfile.Image.Count());
            Assert.IsFalse(UpdatedProfile.IsNew);
            Assert.AreEqual(UpdatedProfile.Id, ReadProfile.Id);
        }

        #region Private Members
        private Profile UpdateProfile(string Firstname, string Surname, string About, int ImageSize)
        {
            var ReadProfile = GetProfile();
            ReadProfile.Firstname = Firstname;
            ReadProfile.Surname = Surname;
            ReadProfile.About = About;
            ReadProfile.Image = new byte[ImageSize];

            var UpdatedProfile = MockDispatcher.Update(
                DataModel: ReadProfile,
                Result: m => m) as Profile;

            //return GetProfile();
            return UpdatedProfile;
        }

        private Profile GetProfile()
        {
            return MockDispatcher.Load<Profile>(
                Id: MockAuthenticationAPI.AuthenticatedId,
                Result: m => m) as Profile;
        }
        #endregion

        #region Initialisation
        [ClassInitialize]
        public static void Initialise(TestContext Context)
        {
            ViewModel.ViewModelFactory.Boot();
        }

        [TestInitialize]
        public void TestInitialise()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();

            MockDispatcher = ViewModelFactory.CreateDispatcher<IDataModel>(
                MockFeedbackAPI as IFeedbackAPI,
                MockAuthenticationAPI as IAuthenticationAPI,
                MockModelAPI as IModelAPI,
                AuthoriseResult: () => null,
                PasswordResetResult: (m) => null,
                DefaultErrorResult: (m) => null);

            // Login and assert success...
            var LoginModel = new Login { EmailAddress = "bart.simpson@skankydog.com", Password = "hello" };
            var Returned = MockDispatcher.Update(
                DataModel: LoginModel,
                SuccessResult: m => m,
                InvalidResult: m => null);

            Assert.IsNotNull(Returned);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();
        }
        #endregion
    }
}
