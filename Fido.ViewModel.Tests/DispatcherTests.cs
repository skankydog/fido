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
    public class AuthenticatedModel : Model<AuthenticatedModel>
    {
        public AuthenticatedModel()
            : base(ReadAccess: Access.Authenticated, WriteAccess: Access.Authenticated)
        { }

        public override AuthenticatedModel Read(Guid Id) { return new AuthenticatedModel(); }
        public override bool Write(AuthenticatedModel Model) { return true; }
        public override bool Confirm(Guid ConfirmationId) { return true; }
    }

    [ExcludeFromCodeCoverage]
    public class PermissionedModel : Model<PermissionedModel>
    {
        public PermissionedModel()
            : base(ReadAccess: Access.Permissioned, WriteAccess: Access.Permissioned)
        { }

        public override PermissionedModel Read(Guid Id) { return new PermissionedModel(); }
        public override bool Write(PermissionedModel Model) { return true; }
        public override bool Confirm(Guid ConfirmationId) { return true; }
    }

    [ExcludeFromCodeCoverage]
    public class AnonymousModel : Model<AnonymousModel>
    {
        public AnonymousModel()
            : base(ReadAccess: Access.NA, WriteAccess: Access.NA)
        { }

        public override AnonymousModel Read(Guid Id) { return new AnonymousModel(); }
        public override bool Write(AnonymousModel Model) { return true; }
        public override bool Confirm(Guid ConfirmationId) { return true; }
    }

    public enum ResultType
    {
        Success = 0,
        Authentication,
        PasswordReset,
        Error,
        Invalid
    }

    [ExcludeFromCodeCoverage]
    public class Result
    {
        public ResultType ResultType;
    }

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DispatcherTests
    {
        private MockFeedbackAPI MockFeedbackAPI = new MockFeedbackAPI();
        private MockAuthenticationAPI MockAuthenticationAPI = new MockAuthenticationAPI();
        private MockModelAPI MockModelAPI = new MockModelAPI();
        private IDispatcher<Result> MockDispatcher;

        [TestMethod]
        public void reading_from_anonymous_models_does_not_require_login()
        {
            CheckRead<AnonymousModel>(ResultType.Success);
        }

        [TestMethod]
        public void reading_from_authenticated_models_requires_login()
        {
            CheckRead<AuthenticatedModel>(ResultType.Authentication);

            Login();
            CheckRead<AuthenticatedModel>(ResultType.Success);
        }

        [TestMethod]
        public void reading_from_permissioned_models_requires_login()
        {
            CheckRead<PermissionedModel>(ResultType.Authentication);

            Login();
            CheckRead<PermissionedModel>(ResultType.Success);
        }

        [TestMethod]
        public void writing_to_anonymous_models_does_not_require_login()
        {
            CheckWrite<AnonymousModel>(ResultType.Success);
        }

        [TestMethod]
        public void writing_to_authenticated_models_requires_login()
        {
            CheckWrite<AuthenticatedModel>(ResultType.Authentication);

            Login();
            CheckWrite<AuthenticatedModel>(ResultType.Success);
        }

        [TestMethod]
        public void writing_to_permissioned_models_requires_login()
        {
            CheckWrite<PermissionedModel>(ResultType.Authentication);

            Login();
            CheckWrite<PermissionedModel>(ResultType.Success);
        }

        #region Helpers
        private void CheckRead<TMODEL>(ResultType ResultToCheck)
            where TMODEL : IModel<TMODEL>
        {
            Assert.AreEqual(ResultToCheck,
                MockDispatcher.Load<TMODEL>(
                Id: Guid.NewGuid(),
                Result: m => new Result { ResultType = ResultType.Success }).ResultType);
        }

        private void CheckWrite<TMODEL>(ResultType ResultToCheck)
            where TMODEL : IModel<TMODEL>
        {
            var Model = (TMODEL)Activator.CreateInstance(typeof(TMODEL));

            var UpdateResult = MockDispatcher.Update(
                DataModel: Model,
                SuccessResult: m => new Result { ResultType = ResultType.Success },
                InvalidResult: m => new Result { ResultType = ResultType.Invalid }).ResultType;
            Assert.AreEqual(ResultToCheck, UpdateResult);

            var CreateResult = MockDispatcher.Create(
                DataModel: Model,
                SuccessResult: m => new Result { ResultType = ResultType.Success },
                InvalidResult: m => new Result { ResultType = ResultType.Invalid }).ResultType;
            Assert.AreEqual(ResultToCheck, CreateResult);
        }
        #endregion

        private void Login()
        {
            var LoginModel = new Login { EmailAddress = "bart.simpson@skankydog.com", Password = "hello" };
            var Returned = MockDispatcher.Update(
                DataModel: LoginModel,
                SuccessResult: m => new Result { ResultType = ResultType.Success },
                InvalidResult: m => new Result { ResultType = ResultType.Invalid }).ResultType;

            Assert.AreEqual(ResultType.Success, Returned);
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

            MockDispatcher = ViewModelFactory.CreateDispatcher<Result>(
                MockFeedbackAPI as IFeedbackAPI,
                MockAuthenticationAPI as IAuthenticationAPI,
                MockModelAPI as IModelAPI,
                AuthoriseResult: () => new Result { ResultType = ResultType.Authentication },
                PasswordResetResult: (m) => new Result { ResultType = ResultType.PasswordReset },
                DefaultErrorResult: (m) => new Result { ResultType = ResultType.Error });
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();
        }
        #endregion
    }
}
