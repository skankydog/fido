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
    public class UserTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;

        [TestMethod]
        public void user_create()
        {
            const string FIRSTNAME = "Firstname";
            const string SURNAME = "Surname";
            const string EMAIL_ADDRESS = "new_email_address@skankydog.com";
            var CreatedUser = MockDispatcher.Create(
                DataModel:
                    new UserCreate
                    {
                        Firstname = FIRSTNAME,
                        Surname = SURNAME,
                        EmailAddress = EMAIL_ADDRESS,
                        Password = "one-off"
                    },
                Result: m => m) as UserCreate;
            var ReadUser = GetUser(CreatedUser.Id);

            Assert.IsNotNull(ReadUser);
            Assert.AreEqual(FIRSTNAME, ReadUser.Firstname);
            Assert.AreEqual(SURNAME, ReadUser.Surname);
            Assert.AreEqual(EMAIL_ADDRESS, ReadUser.EmailAddress);
        }

        [TestMethod]
        public void user_read()
        {
            var FIRSTNAME = "Firstname";
            var SURNAME = "Surname";
            var EMAIL_ADDRESS = "new@skankydog.com";
            var CreatedUser = CreateUser(FIRSTNAME, SURNAME, EMAIL_ADDRESS);

            var ReadUser = MockDispatcher.Load<User>(
                Id: CreatedUser.Id,
                Result: m => m) as User;
            Assert.IsNotNull(ReadUser);
            Assert.AreEqual(FIRSTNAME, ReadUser.Firstname);
            Assert.AreEqual(SURNAME, ReadUser.Surname);
        }

        [TestMethod]
        public void user_update()
        {
            var CREATED_FIRSTNAME = "Created Firstname";
            var CREATED_SURNAME = "Created Surname";
            var CREATED_EMAIL_ADDRESS = "new@skankydog.com";
            var UPDATED_SURNAME = "Updated Surname";

            var ReadUser = CreateUser(CREATED_FIRSTNAME, CREATED_SURNAME, CREATED_EMAIL_ADDRESS);

            ReadUser.Surname = UPDATED_SURNAME;
            ReadUser.SelectedRoles = new List<Guid> { GetRoleId(0), GetRoleId(1) };
            var UpdatedUser = MockDispatcher.Update(
                DataModel: ReadUser,
                Result: m => m) as User;

            UpdatedUser = GetUser(UpdatedUser.Id);

            Assert.IsNotNull(UpdatedUser);
            Assert.AreEqual(UPDATED_SURNAME, UpdatedUser.Surname);
            Assert.AreEqual(2, UpdatedUser.SelectedRoles.Count);
        }

        [TestMethod]
        public void user_list_filtering()
        {
            UserList Result;

            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(11, Result.aaData.Count);

            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "bart.simpson@skankydog.com" },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(1, Result.aaData.Count);

            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "Simpson" },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(3, Result.aaData.Count);

            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "Disabled" },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(2, Result.aaData.Count);

            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "None" },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(1, Result.aaData.Count);
        }

        [TestMethod]
        public void user_list_sorting()
        {
            UserList Result;

            // email address ascending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("", Result.aaData[0][0]);

            // email address decending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "d" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("tony@skankydog.com", Result.aaData[0][0]);

            // firstname ascending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "1" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Bart", Result.aaData[0][1]);

            // firstname decending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "1" }, SortOrders = new List<string> { "d" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Waylan", Result.aaData[0][1]);

            // surname ascending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "2" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Brockman", Result.aaData[0][2]);

            // surname decending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "2" }, SortOrders = new List<string> { "d" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Tony", Result.aaData[0][2]);

            // local credential state ascending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "3" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Disabled", Result.aaData[0][3]);

            // local credential state decending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "3" }, SortOrders = new List<string> { "d" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("None", Result.aaData[0][3]);

            // external credential state ascending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "4" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Disabled", Result.aaData[0][4]);

            // external credential state decending
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "4" }, SortOrders = new List<string> { "d" }, Skip = 0, Take = 99999, Filter = null },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Enabled", Result.aaData[0][4]);
        }

        [TestMethod]
        public void user_list_paging()
        {
            UserList Result;

            // first page of users
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 2 },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(2, Result.aaData.Count);

            // last page of users
            Result = MockDispatcher.List<UserList>(
                IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 2, Take = 99999 },
                Result: m => m) as UserList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(9, Result.aaData.Count);
        }

        #region Private Members
        private User CreateUser(string Firstname, string Surname, string EmailAddress)
        {
            var CreatedUser = MockDispatcher.Create(
                DataModel: new UserCreate { Firstname = Firstname, Surname = Surname, EmailAddress = EmailAddress, Password = "one-off" },
                Result: m => m) as UserCreate;

            return GetUser(CreatedUser.Id);
        }

        private User GetUser(Guid Id)
        {
            var ReadUser = MockDispatcher.Load<User>(
                Id: Id,
                Result: m => m) as User;

            return ReadUser;
        }

        private Guid GetRoleId(int Index)
        {
            var Options = new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null };
            var All = new RoleList().Read(Options);
            var Id = All.aaData[Index][2].ToGuid();

            return Id;
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
