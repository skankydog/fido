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
    public class ActivityTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;

        [TestMethod]
        public void activity_read()
        {
            var ActivityId = GetActivityId();
            var ReadActivity = MockDispatcher.Load<Activity>(
                Id: GetActivityId(),
                Result: m => m) as Activity;

            Assert.IsNotNull(ReadActivity);
            Assert.IsFalse(ReadActivity.IsNew);
            Assert.AreEqual(ActivityId, ReadActivity.Id);
        }

        [TestMethod]
        public void activity_list_filtering()
        {
            ActivityList Result;

            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(6, Result.aaData.Count);
            
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "Namespace 1" },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(3, Result.aaData.Count);
            
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "Model 2" },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(1, Result.aaData.Count);
            
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "Action 1" },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(3, Result.aaData.Count);
        }

        [TestMethod]
        public void activity_list_sorting()
        {
            ActivityList Result;

            // area ascending
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Namespace 1", Result.aaData[0][0]);

            // area decending
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "d" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Namespace 4", Result.aaData[0][0]);

            // name ascending
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "1" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Controller/Model 1", Result.aaData[0][1]);

            // name decending
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "1" }, SortOrders = new List<string> { "d" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Controller/Model 6", Result.aaData[0][1]);

            // read/write ascending
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "2" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Action 0", Result.aaData[0][2]);

            // read/write decending
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "2" }, SortOrders = new List<string> { "d" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Action 9", Result.aaData[0][2]);
        }

        [TestMethod]
        public void activity_list_paging()
        {
            ActivityList Result;

            // first page of activities
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 2 },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(2, Result.aaData.Count);

            // last page of activities
            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 2, Take = 99999 },
                Result: m => m) as ActivityList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(4, Result.aaData.Count);
        }

        #region Private Members
        private Guid GetActivityId()
        {
            ActivityList Result;

            Result = MockDispatcher.List<ActivityList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as ActivityList;
            return (Result.aaData[0][4]).ToGuid();
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
            var LoginModel = new Login { EmailAddress = "homer.simpson@skankydog.com", Password = "hello" };
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
