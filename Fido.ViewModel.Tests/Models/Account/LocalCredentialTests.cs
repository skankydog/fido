using System;
using System.Text;
using System.Collections.Generic;
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
    [TestClass]
    public class LocalCredentialTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;

        [TestMethod]
        public void update_local_credentials_with_duplicate_email_address_generates_error()
        {
            var LocalCredentialModel = new LocalCredential { EmailAddress = "homer.simpson@skankydog.com", Password = Guid.NewGuid().ToString() };
            var Returned = MockDispatcher.Update(
                DataModel: LocalCredentialModel,
                SuccessResult: m => m,
                InvalidResult: m => null);

            Assert.IsNull(Returned);
            Assert.IsTrue(MockModelAPI.HasAnyError);
        }

        [TestMethod]
        public void update_local_credentials_with_invalid_email_address_generates_error()
        {
            var LocalCredentialModel = new LocalCredential { EmailAddress = "invalidemailaddress.com", Password = Guid.NewGuid().ToString() };
            var Returned = MockDispatcher.Update(
                DataModel: LocalCredentialModel,
                SuccessResult: m => m,
                InvalidResult: m => null);

            Assert.IsNull(Returned);
            Assert.IsTrue(MockModelAPI.HasAnyError);
        }

        [TestMethod]
        public void update_local_credentials_with_weak_password_generates_error()
        {
            var LocalCredentialModel = new LocalCredential { EmailAddress = "newemail@skankydog.com", Password = "password" };
            var Returned = MockDispatcher.Update(
                DataModel: LocalCredentialModel,
                SuccessResult: m => m,
                InvalidResult: m => null);

            Assert.IsNull(Returned);
            Assert.IsTrue(MockModelAPI.HasAnyError);
        }

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
