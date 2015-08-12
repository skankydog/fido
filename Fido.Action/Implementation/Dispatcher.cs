using System;
using System.Collections.Generic;
using System.Reflection;
using Fido.Core;

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

        public TRETURN View<TMODEL>(Func<TRETURN> UI)
        {
            using (new FunctionLogger(Log))
            {
                var ModelHandler = GetHandler<TMODEL>();
                var RedirectUI = Check(ModelHandler);

                if (RedirectUI != null)
                    return RedirectUI;

                return UI();
            }
        }

        public TRETURN Read<TMODEL>(Guid Id, int Page, Func<TMODEL, TRETURN> SuccessUI)
        {
            return DoRead<TMODEL>(Id, Page, SuccessUI);
        }

        public TRETURN Read<TMODEL>(Guid Id, Func<TMODEL, TRETURN> SuccessUI)
        {
            return DoRead<TMODEL>(Id, null, SuccessUI);
        }

        private TRETURN DoRead<TMODEL>(Guid Id, int? Page, Func<TMODEL, TRETURN> SuccessUI)
        {
            using (new FunctionLogger(Log))
            {
                var ModelHandler = GetHandler<TMODEL>();
                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, ModelHandler);
                var RedirectUI = Check(ModelHandler);

                if (RedirectUI != null)
                    return RedirectUI;

                return Processor.ExecuteRead(Id, Page, SuccessUI);
            }
        }

        public TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> SuccessUI,
            Func<TMODEL, TRETURN> FailureUI,
            Func<TMODEL, TRETURN> InvalidUI)
        {
            using (new FunctionLogger(Log))
            {
                var ModelHandler = GetHandler<TMODEL>();
                var Processor = new Processor<TMODEL, TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, ModelHandler);
                var RedirectUI = Check(ModelHandler);

                if (RedirectUI != null)
                    return RedirectUI;

                return Processor.ExecuteWrite(Model, SuccessUI, FailureUI, InvalidUI);
            }
        }

        public TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> UI)
        {
            return Write(Model, UI, UI, UI);
        }

        private IModel<TMODEL> GetHandler<TMODEL>()
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

        private TRETURN Check<TMODEL>(IModel<TMODEL> Handler)
        {
            if (Handler.RequiresAuthentication)
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
