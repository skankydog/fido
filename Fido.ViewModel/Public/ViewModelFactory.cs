using System;
using System.Collections.Generic;
using Fido.Core.Bootstrapper;
using Fido.ViewModel.Implementation;

namespace Fido.ViewModel
{
    public static class ViewModelFactory
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
            Func<IDataModel, TRETURN> PasswordResetResult,
            Func<IDataModel, TRETURN> DefaultErrorResult) where TRETURN : class
        {
            return new Dispatcher<TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI, AuthoriseResult, PasswordResetResult, DefaultErrorResult);
        }
    }
}
