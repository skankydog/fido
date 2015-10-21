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
            Func<TRETURN> AuthoriseResult,
            Func<TRETURN> PasswordResetResult)
        {
            return new Dispatcher<TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, AuthoriseResult, PasswordResetResult);
        }
    }
}
