using System;
using System.Collections.Generic;
using System.Reflection;
using Fido.Core;
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

        public TRETURN View<TMODEL>(Func<TRETURN> Result)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = GetLogicModel<TMODEL>();
                var Redirect = CheckPermission(LogicModel); // Write

                if (Redirect != null)
                    return Redirect;

                return Result();
            }
        }

        public TRETURN Read<TMODEL>(Guid Id, IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result)
        {
            return DoRead(Id, IndexOptions, Result);
        }

        public TRETURN Read<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result)
        {
            return DoRead<TMODEL>(Id, null, Result);
        }

        private TRETURN DoRead<TMODEL>(Guid Id, IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = GetLogicModel<TMODEL>();
                var Redirect = CheckPermission(LogicModel); // Read

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteRead(Id, IndexOptions, Result);
            }
        }

        public TRETURN Write<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> FailureResult,
            Func<TMODEL, TRETURN> InvalidResult)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = GetLogicModel<TMODEL>();
                var Redirect = CheckPermission(LogicModel); // Write

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteWrite(DataModel, SuccessResult, FailureResult, InvalidResult);
            }
        }

        public TRETURN Write<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> AnyResult)
        {
            return Write(DataModel, AnyResult, AnyResult, AnyResult);
        }

        public TRETURN Delete_<TMODEL>(TMODEL DataModel, Func<TRETURN> Result)
        {
            using (new FunctionLogger(Log))
            {
                var LogicModel = GetLogicModel<TMODEL>();
                var Redirect = CheckPermission(LogicModel); // Delete

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, LogicModel);
                return Processor.ExecuteDelete(DataModel, Result);
            }
        }

        private IModel<TMODEL> GetLogicModel<TMODEL>()
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

        private TRETURN CheckPermission<TMODEL>(IModel<TMODEL> LogicModel)
        {
            if (LogicModel.RequiresAuthentication)
            {
                if (!AuthenticationAPI.Authenticated)
                    return AuthenticateResult();

                if (AuthenticationAPI.LoggedInCredentialState == "Expired")
                    return PasswordResetResult();

                // PermissionCheck(UserInterface.UserId, ReadHandler.Name, Write) // throw if missing required permission
            }

            return default(TRETURN);
        }
    }
}
