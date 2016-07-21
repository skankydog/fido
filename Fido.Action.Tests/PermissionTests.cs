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
using Fido.Action.Tests.Mocks;

namespace Fido.Action.Tests
{
    [TestClass]
    public class PermissionTests
    {
        private IFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private IAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private IModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;

        [TestMethod]
        public void Permissions()
        {
            var Result = MockDispatcher.Load<Profile>(MockAuthenticationAPI.AuthenticatedId, (m) => m);

            Assert.IsTrue(Result.HasReadOrWritePermission("Activity04", "Area 2"));        // valid actiity, valid area
            Assert.IsFalse(Result.HasReadOrWritePermission("non existent", "Area 2"));     // invalid activity, valid area
            Assert.IsFalse(Result.HasReadOrWritePermission("Activity04", "non existent")); // valid activity, invalid area
            Assert.IsFalse(Result.HasReadOrWritePermission("Activity05", "Area 3"));       // valid actvity, valid area - does not have permission

            Assert.IsTrue(Result.HasWritePermission("Activity03", "Area 1"));              // valid activity, valid area
            Assert.IsFalse(Result.HasWritePermission("non existent", "Area 1"));           // invalid activity, valid area
            Assert.IsFalse(Result.HasWritePermission("Activity03", "non existent"));       // valid activity, invalid area
            Assert.IsFalse(Result.HasWritePermission("Activity01", "Area 1"));             // valid activity, valid area - does not have permission

            Assert.IsTrue(Result.HasReadPermission("Activity01", "Area 1"));               // valid activity, valid area
            Assert.IsFalse(Result.HasReadPermission("non existent", "Area 1"));            // invalid activity, valid area
            Assert.IsFalse(Result.HasReadPermission("Activity03", "non existent"));        // valid activity, invalid area
            Assert.IsFalse(Result.HasReadPermission("Activity02", "Area 1"));              // valid activity, valid area - does not have permission

            Assert.IsTrue(Result.HasArea("Area 2"));
            Assert.IsFalse(Result.HasArea("Area 4"));
            Assert.IsFalse(Result.HasArea("non existent"));
        }

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
                MockFeedbackAPI,
                MockAuthenticationAPI,
                MockModelAPI,
                AuthoriseResult: () => null,
                PasswordResetResult: (m) => null,
                DefaultErrorResult: (m) => null);

            // Login and assert success...
            var Credentials = new Login { EmailAddress = "bart.simpson@skankydog.com", Password = "hello" };
            var Result = MockDispatcher.Save(
                DataModel: Credentials,
                SuccessResult: m => m,
                InvalidResult: m => null);

            Assert.IsNotNull(Result);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();
        }
    }
}
