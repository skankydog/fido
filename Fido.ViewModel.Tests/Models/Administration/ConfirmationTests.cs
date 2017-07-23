using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Service;
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
    public class ConfirmationTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;

        [TestMethod]
        public void confirmation_read()
        {
            var CreatedConfirmation = CreateConfirmation();

            var ReadConfirmation = MockDispatcher.Load<Confirmation>(
                Id: CreatedConfirmation.Id,
                Result: m => m) as Confirmation;
            Assert.IsNotNull(ReadConfirmation);
            Assert.IsFalse(ReadConfirmation.IsNew);
            Assert.AreEqual(CreatedConfirmation.Id, ReadConfirmation.Id);
            Assert.IsNull(ReadConfirmation.SentUTC);
            Assert.IsNull(ReadConfirmation.ReceivedUTC);
        }

        [TestMethod]
        public void confirmation_delete()
        {
            var CreatedConfirmation = CreateConfirmation();

            var DeletedConfirmation = MockDispatcher.DeleteIt<Confirmation>(
                DataModel: CreatedConfirmation,
                Result: m => m) as Confirmation;

            var ReadConfirmation = MockDispatcher.Load<Confirmation>(
                Id: CreatedConfirmation.Id,
                Result: m => m) as Confirmation;
            Assert.IsNull(ReadConfirmation);
        }

        [TestMethod]
        public void confirmation_list_filtering()
        {
            ConfirmationList Result;
            var UserId = MockAuthenticationAPI.AuthenticatedId;

            Result = MockDispatcher.List<ConfirmationList>(
                IndexOptions: new ListOptions { Id = UserId, SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as ConfirmationList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(7, Result.aaData.Count);

            Result = MockDispatcher.List<ConfirmationList>(
                IndexOptions: new ListOptions { Id = UserId, SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "bart.simpson@skankydog.com" },
                Result: m => m) as ConfirmationList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(5, Result.aaData.Count);

            Result = MockDispatcher.List<ConfirmationList>(
                IndexOptions: new ListOptions { Id = UserId, SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "Change Email Address" },
                Result: m => m) as ConfirmationList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(3, Result.aaData.Count);
        }

        [TestMethod]
        public void confirmation_list_paging()
        {
            ConfirmationList Result;
            var UserId = MockAuthenticationAPI.AuthenticatedId;

            // first page of confirmations...
            Result = MockDispatcher.List<ConfirmationList>(
                IndexOptions: new ListOptions { Id = UserId, SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 2 },
                Result: m => m) as ConfirmationList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(2, Result.aaData.Count);

            // last page of confirmations...
            Result = MockDispatcher.List<ConfirmationList>(
                IndexOptions: new ListOptions { Id = UserId, SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 2, Take = 99999 },
                Result: m => m) as ConfirmationList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(5, Result.aaData.Count);
        }

        #region Private Members
        private Confirmation CreateConfirmation()
        {
            var NewEmailAddress = new EmailAddress
            {
                Email = "new@skankydog.com",
            };

            var ChangedEmailAddress = MockDispatcher.Update(
                DataModel: NewEmailAddress,
                Result: m => m) as EmailAddress;

            var CreatedConfirmaton = MockDispatcher.Load<Confirmation>(
                Id: ChangedEmailAddress.ConfirmationId,
                Result: m => m) as Confirmation;

            return CreatedConfirmaton;
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
