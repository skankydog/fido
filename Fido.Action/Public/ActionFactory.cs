using System;
using System.Collections.Generic;
using Fido.Core.Bootstrapper;
using Fido.Action.Implementation;

namespace Fido.Action
{
    public static class ActionFactory
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Boot()
        {
            BootstrapperEngine.Bootstrap();
        }

        public static IDispatcher<TRETURN> CreateDispatcher<TRETURN>(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI AuthenticationAPI,
            IModelAPI ModelAPI,
            Func<TRETURN> AuthoriseUI,
            Func<TRETURN> PasswordResetUI)
        {
            return new Dispatcher<TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, AuthoriseUI, PasswordResetUI);
        }
    }
}
