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
    public class PermissionTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;
        private IDataModel LoadedModel;

        [TestMethod]
        public void Permissions()
        {
            //Assert.IsTrue(LoadedModel.Allowed("Action 1", "Controller/Model 1", "Namespace 1"));
            //Assert.IsTrue(LoadedModel.Allowed("Action 9", "Controller/Model 2", "Namespace 1"));
            //Assert.IsTrue(LoadedModel.Allowed("Action 2", "Controller/Model 3", "Namespace 1"));
            //Assert.IsTrue(LoadedModel.Allowed("Action 1", "Controller/Model 4", "Namespace 2"));
            //Assert.IsFalse(LoadedModel.Allowed("Action 1", "Controller/Model 5", "Namespace 3"));
            //Assert.IsFalse(LoadedModel.Allowed("Action 0", "Controller/Model 6", "Namespace 4"));
            //Assert.IsTrue(LoadedModel.Allowed("None", "None", "None"));

            Assert.IsFalse(LoadedModel.Denied.Contains(string.Concat("Action 1", ".", "Controller/Model 1", ".", "Namespace 1")));
            Assert.IsFalse(LoadedModel.Denied.Contains(string.Concat("Action 9", ".", "Controller/Model 2", ".", "Namespace 1")));
            Assert.IsFalse(LoadedModel.Denied.Contains(string.Concat("Action 2", ".", "Controller/Model 3", ".", "Namespace 1")));
            Assert.IsFalse(LoadedModel.Denied.Contains(string.Concat("Action 1", ".", "Controller/Model 4", ".", "Namespace 2")));
            Assert.IsFalse(LoadedModel.Denied.Contains(string.Concat("None", ".", "None", ".", "None")));
            Assert.IsTrue(LoadedModel.Denied.Contains(string.Concat("Action 1", ".", "Controller/Model 5", ".", "Namespace 3")));
            Assert.IsTrue(LoadedModel.Denied.Contains(string.Concat("Action 0", ".", "Controller/Model 6", ".", "Namespace 4")));
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
            var Credentials = new Login { EmailAddress = "bart.simpson@skankydog.com", Password = "hello" };
            LoadedModel = MockDispatcher.Update(
                DataModel: Credentials,
                SuccessResult: m => m,
                InvalidResult: m => null);

            Assert.IsNotNull(LoadedModel);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();
        }
        #endregion
    }
}
