using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Action;
using Fido.Action.Implementation;
using Fido.Action.Models;
using Fido.Action.Models.Account;
using Fido.Action.Models.Administration;
using Fido.Action.Models.Authentication;
using Fido.Action.Tests.Mocks;

namespace Fido.Action.Tests
{
    [TestClass]
    public class EmailAddressTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;

        [TestMethod]
        public void DuplicateEmailAddressGeneratesError()
        {
            var EmailAddressModel = new EmailAddress { Email = "homer.simpson@skankydog.com" };
            var Returned = MockDispatcher.Update(
                DataModel: EmailAddressModel,
                SuccessResult: m => m,
                InvalidResult: m => null);

            Assert.IsNull(Returned);
            Assert.IsTrue(MockModelAPI.HasAnyError);
        }

        [TestMethod]
        public void InvalidEmailAddressGeneratesError()
        {
            var EmailAddressModel = new EmailAddress { Email = "invalidemailaddress.com" };
            var Returned = MockDispatcher.Update(
                DataModel: EmailAddressModel,
                SuccessResult: m => m,
                InvalidResult: m => null);

            Assert.IsNull(Returned);
            Assert.IsTrue(MockModelAPI.HasAnyError);
        }

        [TestMethod]
        public void CanChangeEmailAddress()
        {
            var EmailAddressModel = new EmailAddress { Email = "valid.email@skankydog.com" };
            var Returned = MockDispatcher.Update(
                DataModel: EmailAddressModel,
                SuccessResult: m => m,
                InvalidResult: m => null);

            Assert.IsNotNull(Returned);
            Assert.IsFalse(MockModelAPI.HasAnyError);
        }

        #region Initialisation
        [ClassInitialize]
        public static void Initialise(TestContext Context)
        {
            Action.ActionFactory.Boot();
        }

        [TestInitialize]
        public void TestInitialise()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();

            MockDispatcher = ActionFactory.CreateDispatcher<IDataModel>(
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
