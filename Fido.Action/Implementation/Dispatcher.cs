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
        Func<TRETURN> AuthenticateUI;
        Func<TRETURN> PasswordResetUI;

        public Dispatcher(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI AuthenticationAPI,
            IModelAPI ModelAPI,
            Func<TRETURN> AuthoriseUI,
            Func<TRETURN> PasswordResetUI)
        {
            this.FeedbackAPI = FeedbackAPI;
            this.AuthenticationAPI = AuthenticationAPI;
            this.ModelAPI = ModelAPI;
            this.AuthenticateUI = AuthoriseUI;
            this.PasswordResetUI = PasswordResetUI;
        }

        public TRETURN View<TMODEL>(Func<TRETURN> Any)
        {
            using (new FunctionLogger(Log))
            {
                var ModelInstance = GetModel<TMODEL>();
                var RedirectUI = CheckPermission(ModelInstance);

                if (RedirectUI != null)
                    return RedirectUI;

                return Any();
            }
        }

        public TRETURN Read<TMODEL>(Guid Id, IndexParams Params, Func<TMODEL, TRETURN> Success)
        {
            return DoRead(Id, Params, Success);
        }

        public TRETURN Read<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Success)
        {
            return DoRead<TMODEL>(Id, null, Success);
        }

        private TRETURN DoRead<TMODEL>(Guid Id, IndexParams Params, Func<TMODEL, TRETURN> Success)
        {
            using (new FunctionLogger(Log))
            {
                var ModelInstance = GetModel<TMODEL>();
                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, ModelInstance);
                var RedirectUI = CheckPermission(ModelInstance);

                if (RedirectUI != null)
                    return RedirectUI;

                return Processor.ExecuteRead(Id, Params, Success);
            }
        }

        public TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> Success,
            Func<TMODEL, TRETURN> Failure,
            Func<TMODEL, TRETURN> Invalid)
        {
            using (new FunctionLogger(Log))
            {
                var ModelInstance = GetModel<TMODEL>();
                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, ModelInstance);
                var RedirectUI = CheckPermission(ModelInstance);

                if (RedirectUI != null)
                    return RedirectUI;

                return Processor.ExecuteWrite(Model, Success, Failure, Invalid);
            }
        }

        public TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> Any)
        {
            return Write(Model, Any, Any, Any);
        }

        public TRETURN Delete_<TMODEL>(Guid Id, Func<TRETURN> Success)
        {
            using (new FunctionLogger(Log))
            {
                var ModelInstance = GetModel<TMODEL>();
                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, ModelInstance);
                var Redirect = CheckPermission(ModelInstance);

                if (Redirect != null)
                    return Redirect;

                return Processor.ExecuteDelete(Id, Success);
            }
        }

        private IModel<TMODEL> GetModel<TMODEL>()
        {
            using (new FunctionLogger(Log))
            {
                var SourceAssembly = Assembly.GetAssembly(this.GetType());
                var HandlerPath = string.Concat(SourceAssembly.GetName().Name, ".Models.", typeof(TMODEL).Name);
                var HandlerType = SourceAssembly.GetType(HandlerPath);

                if (HandlerType == null)
                    throw new Exception(string.Format("{0} <T> not found", HandlerPath));

                return (IModel<TMODEL>)Activator.CreateInstance(HandlerType, FeedbackAPI, AuthenticationAPI, ModelAPI);
            }
        }

        private TRETURN CheckPermission<TMODEL>(IModel<TMODEL> Model)
        {
            if (Model.RequiresAuthentication)
            {
                if (!AuthenticationAPI.Authenticated)
                    return AuthenticateUI();

                if (AuthenticationAPI.LoggedInCredentialState == "Expired")
                    return PasswordResetUI();

                // PermissionCheck(UserInterface.UserId, ReadHandler.Name, Write) // throw if missing required permission
            }

            return default(TRETURN);
        }
    }
}
