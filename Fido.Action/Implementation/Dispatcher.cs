using System;
using System.Collections.Generic;
using System.Reflection;
using Fido.Core;
using Fido.Service;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    public class Dispatcher<TRETURN> : IDispatcher<TRETURN>
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IFeedbackAPI FeedbackAPI;
        IAuthenticationAPI AuthenticationAPI;
        IModelAPI ModelAPI;
        Func<TRETURN> AuthenticateResult;
        Func<TRETURN> PasswordResetResult;
        Func<TRETURN> DefaultErrorResult;

        public Dispatcher(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI AuthenticationAPI,
            IModelAPI ModelAPI,
            Func<TRETURN> AuthenticateResult,
            Func<TRETURN> PasswordResetResult,
            Func<TRETURN> DefaultErrorResult)
        {
            this.FeedbackAPI = FeedbackAPI;
            this.AuthenticationAPI = AuthenticationAPI;
            this.ModelAPI = ModelAPI;
            this.AuthenticateResult = AuthenticateResult;
            this.PasswordResetResult = PasswordResetResult;
            this.DefaultErrorResult = DefaultErrorResult;
        }

        #region Index
        public TRETURN ReturnIndexWrapper<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
        {
            return ReturnModelOrView(
                DataModel: DataModel,
                SuccessResult: Result,
                ErrorResult: DefaultErrorResult,
                RequiredPermission: Permission.Read);
        }

        public TRETURN ReturnIndexWrapper<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            return ReturnModelOrView(
                DataModel: DataModel,
                SuccessResult: SuccessResult,
                ErrorResult: ErrorResult,
                RequiredPermission: Permission.Read);
        }
        #endregion

        #region EmptyModel
        public TRETURN ReturnEmptyModel<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
        {
            return ReturnModelOrView(
                DataModel: DataModel,
                SuccessResult: Result,
                ErrorResult: DefaultErrorResult,
                RequiredPermission: Permission.Write);
        }

        public TRETURN ReturnEmptyModel<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            return ReturnModelOrView(
                DataModel: DataModel,
                SuccessResult: SuccessResult,
                ErrorResult: ErrorResult,
                RequiredPermission: Permission.Write);
        }
        #endregion

        private TRETURN ReturnModelOrView<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult, Permission RequiredPermission)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = CreateLogicModel<TMODEL>();
                var Redirect = CheckForPermission(LogicModel, RequiredPermission);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteView(
                    DataModel: DataModel,
                    SuccessResult: SuccessResult,
                    ErrorResult: ErrorResult);
            }
        }

        #region Reads
        public TRETURN ReturnLoadedModel<TMODEL>(IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result)
        {
            return ReturnLoadedModel(
                IndexOptions: IndexOptions,
                SuccessResult: Result,
                ErrorResult: DefaultErrorResult);
        }

        public TRETURN ReturnLoadedModel<TMODEL>(IndexOptions IndexOptions, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = CreateLogicModel<TMODEL>();
                var Redirect = CheckForPermission(LogicModel, Permission.Read);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteRead(
                    IndexOptions: IndexOptions,
                    SuccessResult: SuccessResult,
                    ErrorResult: ErrorResult);
            }
        }

        public TRETURN ReturnLoadedModel<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result)
        {
            return ReturnLoadedModel(
                Id: Id,
                SuccessResult: Result,
                ErrorResult: DefaultErrorResult);
        }

        public TRETURN ReturnLoadedModel<TMODEL>(Guid Id, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = CreateLogicModel<TMODEL>();
                var Redirect = CheckForPermission(LogicModel, Permission.Write); // TO DO?: If they have read access, they should be able to view the record, just not save/write (Update post).

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteRead(
                    Id: Id,
                    SuccessResult: SuccessResult,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Save
        public TRETURN SavePostedModel<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
        {
            return SavePostedModel(
                DataModel: DataModel,
                SuccessResult: Result,
                InvalidResult: Result,
                ErrorResult: DefaultErrorResult);
        }

        public TRETURN SavePostedModel<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult)
        {
            return SavePostedModel(
                DataModel: DataModel,
                SuccessResult: SuccessResult,
                InvalidResult: InvalidResult,
                ErrorResult: DefaultErrorResult);
        }

        public TRETURN SavePostedModel<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult,
            Func<TRETURN> ErrorResult)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = CreateLogicModel<TMODEL>();
                var Redirect = CheckForPermission(LogicModel, Permission.Write);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteWrite(
                    DataModel: DataModel,
                    SuccessResult: SuccessResult,
                    InvalidResult: InvalidResult,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Delete
        public TRETURN DeletePostedModel<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
        {
            return DeletePostedModel(
                DataModel: DataModel,
                SuccessResult: Result,
                ErrorResult: DefaultErrorResult);
        }

        public TRETURN DeletePostedModel<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = CreateLogicModel<TMODEL>();
                var Redirect = CheckForPermission(LogicModel, Permission.Write);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteDelete(
                    DataModel: DataModel,
                    SuccessResult: SuccessResult,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Private Members
        private IModel<TMODEL> CreateLogicModel<TMODEL>()
        {
            using (new FunctionLogger(Log))
            {
                var SourceAssembly = Assembly.GetAssembly(this.GetType());
                var ModelPath = string.Concat(SourceAssembly.GetName().Name, ".Models.", typeof(TMODEL).Name);
                var ModelType = SourceAssembly.GetType(ModelPath);

                if (ModelType == null)
                    throw new Exception(string.Format("{0} <T> not found", ModelPath));

                return (IModel<TMODEL>)Activator.CreateInstance(ModelType, FeedbackAPI, AuthenticationAPI, ModelAPI);
            }
        }

        private TRETURN CheckForPermission<TMODEL>(IModel<TMODEL> LogicModel, Permission Permission)
        {
            using (new FunctionLogger(Log))
            {
                if (LogicModel.RequiresReadPermission && Permission == Permission.Read ||
                    LogicModel.RequiresWritePermission && Permission == Permission.Write)
                {
                    if (!AuthenticationAPI.Authenticated)
                        return AuthenticateResult();

                    if (AuthenticationAPI.LoggedInCredentialState == "Expired")
                        return PasswordResetResult();

                    var UserService = ServiceFactory.CreateService<IUserService>();
                    var Name = string.Format("{0}-{1}", LogicModel.GetType().Name, Permission.ToString());

                    if (!UserService.UserHasActivity(AuthenticationAPI.AuthenticatedId, Name))
                    {
                        FeedbackAPI.DisplayError("You are not authorised to perform the requested action.");
                        return AuthenticateResult();
                    }
                }

                return default(TRETURN);
            }
        }
        #endregion
    }
}
