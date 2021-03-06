﻿using System;
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
    public class ExternalCredentialTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<IDataModel> MockDispatcher;

        [TestMethod]
        public void add_external_credentials_to_user()
        {
            var CountBefore = GetExternalCredentials().Count;

            var NewExternalCredential = new ExternalCredential { Id = Guid.Empty, LoginProvider = "BookFace", ProviderKey = "some key", EmailAddress = "new.email@skankydog.com" };
            var Result = MockDispatcher.Update(
                DataModel: NewExternalCredential,
                SuccessResult: m => m,
                InvalidResult: m => null) as ExternalCredential;

            Assert.IsNotNull(Result);
            Assert.IsFalse(MockModelAPI.HasAnyError);
            
            var CountAfter = GetExternalCredentials().Count;
            Assert.AreEqual(CountBefore + 1, CountAfter);
        }

        [TestMethod]
        public void remove_external_credentials_from_user()
        {
            var ExternalCredentials = GetExternalCredentials();
            var CountBefore = ExternalCredentials.Count;
            Assert.IsTrue(CountBefore > 0);

            var FirstExternalCredential = ExternalCredentials.FirstOrDefault();
            Assert.IsNotNull(FirstExternalCredential);

            var Result = MockDispatcher.Update(
                DataModel: FirstExternalCredential,
                SuccessResult: m => m,
                InvalidResult: m => null) as ExternalCredential;
            Assert.IsNotNull(Result);
            Assert.IsFalse(MockModelAPI.HasAnyError);

            var CountAfter = GetExternalCredentials().Count;
            Assert.AreEqual(CountBefore - 1, CountAfter);
        }

        #region Private Members
        private IList<ExternalCredential> GetExternalCredentials()
        {
            var Result = MockDispatcher.Load<Settings>(
                Id: MockAuthenticationAPI.AuthenticatedId,
                Result: m => m) as Settings;

            return Result.ExternalCredentials;
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
