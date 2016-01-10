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

        public Dispatcher(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI AuthenticationAPI,
            IModelAPI ModelAPI,
            Func<TRETURN> AuthenticateResult,
            Func<TRETURN> PasswordResetResult)
        {
            this.FeedbackAPI = FeedbackAPI;
            this.AuthenticationAPI = AuthenticationAPI;
            this.ModelAPI = ModelAPI;
            this.AuthenticateResult = AuthenticateResult;
            this.PasswordResetResult = PasswordResetResult;
        }

        public TRETURN ReturnEmptyModel<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
        {
            return ReturnModelOrView(DataModel, Result, Permission.Write);
        }

        public TRETURN ReturnIndexWrapper<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
        {
            return ReturnModelOrView(DataModel, Result, Permission.Read);
        }

        private TRETURN ReturnModelOrView<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result, Permission RequiredPermission)
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
                    Result: Result);
            }
        }

        public TRETURN ReturnLoadedModel<TMODEL>(IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = CreateLogicModel<TMODEL>();
                var Redirect = CheckForPermission(LogicModel, Permission.Read);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteRead(IndexOptions, Result);
            }
        }

        public TRETURN ReturnLoadedModel<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = CreateLogicModel<TMODEL>();
                var Redirect = CheckForPermission(LogicModel, Permission.Write); // TO DO?: If they have read access, they should be able to view the record, just not save/write (Update post).

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteRead(Id, Result);
            }
        }

        #region Save
        public TRETURN SavePostedModel<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> NonsuccessResult)
        {
            return SavePostedModel(
                DataModel: DataModel,
                SuccessResult: SuccessResult,
                FailureResult: NonsuccessResult,
                InvalidResult: NonsuccessResult);
        }

        public TRETURN SavePostedModel<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> FailureResult,
            Func<TMODEL, TRETURN> InvalidResult)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = CreateLogicModel<TMODEL>();
                var Redirect = CheckForPermission(LogicModel, Permission.Write);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteWrite(DataModel, SuccessResult, FailureResult, InvalidResult);
            }
        }
        #endregion

        #region Delete
        public TRETURN DeletePostedModel<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = CreateLogicModel<TMODEL>();
                var Redirect = CheckForPermission(LogicModel, Permission.Write);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteDelete(DataModel, Result);
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
