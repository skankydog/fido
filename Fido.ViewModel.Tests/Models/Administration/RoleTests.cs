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
    public class RoleTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;

        [TestMethod]
        public void role_create()
        {
            var NAME = "Role Name";
            var CreatedRole = MockDispatcher.Create(
                DataModel:
                    new Role
                    {
                        Name = NAME,
                        SelectedActivities = new List<Guid> { GetActivityId(0), GetActivityId(1) }
                    },
                Result: m => m) as Role;
            var ReadRole = GetRole(CreatedRole.Id);
            
            Assert.AreEqual(NAME, ReadRole.Name);
            Assert.AreEqual(2, ReadRole.SelectedActivities.Count);
            Assert.IsFalse(ReadRole.IsNew);
            Assert.AreEqual(CreatedRole.Id, ReadRole.Id);
        }

        [TestMethod]
        public void role_read()
        {
            var NAME = "Role Name";
            var CreatedRole = CreateRole(NAME);

            var ReadRole = MockDispatcher.Load<Role>(
                Id: CreatedRole.Id,
                Result: m => m) as Role;
            Assert.IsNotNull(ReadRole);
            Assert.AreEqual(NAME, ReadRole.Name);
            Assert.AreEqual(2, ReadRole.SelectedActivities.Count);
            Assert.IsFalse(ReadRole.IsNew);
            Assert.AreEqual(CreatedRole.Id, ReadRole.Id);
        }

        [TestMethod]
        public void role_update()
        {
            var CREATED_NAME = "Created Role Name";
            var UPDATED_NAME = "Updated Role Name";

            var CreatedRole = CreateRole(CREATED_NAME);

            CreatedRole.Name = UPDATED_NAME;
            var UpdatedRole = MockDispatcher.Update(
                DataModel: CreatedRole,
                Result: m => m) as Role;

            UpdatedRole = GetRole(CreatedRole.Id);

            Assert.IsNotNull(UpdatedRole);
            Assert.AreEqual(UPDATED_NAME, UpdatedRole.Name);
            Assert.AreEqual(2, UpdatedRole.SelectedActivities.Count);
            Assert.IsFalse(UpdatedRole.IsNew);
            Assert.AreEqual(CreatedRole.Id, UpdatedRole.Id);
        }

        [TestMethod]
        public void role_list_filtering()
        {
            RoleList Result;

            Result = MockDispatcher.List<RoleList>(
                        IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                        Result: m => m) as RoleList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(4, Result.aaData.Count);

            Result = MockDispatcher.List<RoleList>(
                        IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = "Role" },
                        Result: m => m) as RoleList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(2, Result.aaData.Count);
        }

        [TestMethod]
        public void role_list_sorting()
        {
            RoleList Result;

            // name ascending
            Result = MockDispatcher.List<RoleList>(
                        IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null },
                        Result: m => m) as RoleList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Administrator", Result.aaData[0][0]);

            // name decending
            Result = MockDispatcher.List<RoleList>(
                        IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "d" }, Skip = 0, Take = 99999, Filter = null },
                        Result: m => m) as RoleList;
            Assert.IsNotNull(Result);
            Assert.AreEqual("Role 3", Result.aaData[0][0]);
        }

        [TestMethod]
        public void role_list_paging()
        {
            RoleList Result;

            // get first page of roles...
            Result = MockDispatcher.List<RoleList>(
                        IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 2 },
                        Result: m => m) as RoleList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(2, Result.aaData.Count);

            // get last page of roles...
            Result = MockDispatcher.List<RoleList>(
                        IndexOptions: new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 2, Take = 99999 },
                        Result: m => m) as RoleList;
            Assert.IsNotNull(Result);
            Assert.AreEqual(2, Result.aaData.Count);
        }

        #region Private Members
        Role CreateRole(string Name)
        {
            var ACTIVITIES = new List<Guid> { GetActivityId(0), GetActivityId(1) };

            var CreatedRole = MockDispatcher.Create(
                DataModel: new Role { Name = Name, SelectedActivities = ACTIVITIES },
                Result: m => m) as Role;

            return GetRole(CreatedRole.Id);
        }

        Role GetRole(Guid Id)
        {
            var ReadRole = MockDispatcher.Load<Role>(
                Id: Id,
                Result: m => m) as Role;

            return ReadRole;
        }

        Guid GetActivityId(int Index)
        {
            var Options = new ListOptions { SortColumns = new List<string> { "0" }, SortOrders = new List<string> { "a" }, Skip = 0, Take = 99999, Filter = null };
            var All = new ActivityList().Read(Options);
            var Id = All.aaData[Index][3].ToGuid();

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
