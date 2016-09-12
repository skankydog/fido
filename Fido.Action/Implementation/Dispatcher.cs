using System;
using System.Collections.Generic;
using System.Linq;
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
        Func<IDataModel, TRETURN> PasswordResetResult;
        Func<IDataModel, TRETURN> ErrorResult;

        public Dispatcher(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI AuthenticationAPI,
            IModelAPI ModelAPI,
            Func<TRETURN> AuthenticateResult,
            Func<IDataModel, TRETURN> PasswordResetResult,
            Func<IDataModel, TRETURN> ErrorResult)
        {
            this.FeedbackAPI = FeedbackAPI;
            this.AuthenticationAPI = AuthenticationAPI;
            this.ModelAPI = ModelAPI;
            this.AuthenticateResult = AuthenticateResult;
            this.PasswordResetResult = PasswordResetResult;
            this.ErrorResult = ErrorResult;
        }

        #region View/Create
        public TRETURN View<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            return DoView(
                DataModel: DataModel,
                SuccessResult: Result,
                RequestedAction: Action.Read);
        }

        public TRETURN View(Func<NoModel, TRETURN> Result)
        {
            return DoView<NoModel>(
                DataModel: new NoModel(),
                SuccessResult: Result,
                RequestedAction: Action.Read);
        }

        public TRETURN Create<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            return DoView(
                DataModel: DataModel,
                SuccessResult: Result,
                RequestedAction: Action.Write);
        }

        private TRETURN DoView<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Action RequestedAction)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = Build(DataModel);
                var Redirect = Check(Model, RequestedAction);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI);
                return Processor.ExecuteView(
                    DataModel: Model,
                    SuccessResult: SuccessResult,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Load
        public TRETURN Load<TMODEL>(IndexOptions IndexOptions, Func<TMODEL, TRETURN> SuccessResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = Build<TMODEL>();
                var Redirect = Check(Model, Action.Read);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI);
                return Processor.ExecuteRead<TMODEL>(
                    DataModel: Model,
                    IndexOptions: IndexOptions,
                    SuccessResult: SuccessResult,
                    ErrorResult: ErrorResult);
            }
        }

        public TRETURN Load<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = Build<TMODEL>();
                var Redirect = Check(Model, Action.Read);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI);
                return Processor.ExecuteRead<TMODEL>(
                    Id: Id,
                    DataModel: Model,
                    SuccessResult: Result,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        public TRETURN Confirm<TMODEL>(Guid ConfirmationId, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = Build<TMODEL>();

                var Processor = new Processor<TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI);
                return Processor.ExecuteConfirm<TMODEL>(
                    ConfirmationId: ConfirmationId,
                    DataModel: Model,
                    SuccessResult: Result,
                    ErrorResult: ErrorResult);
            }
        }

        #region Save
        public TRETURN Save<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            return Save(
                DataModel: DataModel,
                SuccessResult: Result,
                InvalidResult: Result);
        }

        public TRETURN Save<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult)
                where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = Build(DataModel);
                var Redirect = Check(Model, Action.Write);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI);
                return Processor.ExecuteWrite(
                    DataModel: Model,
                    SuccessResult: SuccessResult,
                    InvalidResult: InvalidResult,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Delete
        public TRETURN Delete<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = Build(DataModel);
                var Redirect = Check(Model, Action.Write);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TRETURN>(FeedbackAPI, AuthenticationAPI, ModelAPI);
                return Processor.ExecuteDelete(
                    DataModel: Model,
                    SuccessResult: SuccessResult,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Helpers
        private TMODEL Build<TMODEL>()
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var SourceAssembly = Assembly.GetAssembly(this.GetType());
                var ModelPath = typeof(TMODEL).FullName;
                var ModelType = SourceAssembly.GetType(ModelPath);

                if (ModelType == null)
                    throw new Exception(string.Format("{0} <T> not found", ModelPath));

                var Data = (TMODEL)Activator.CreateInstance(ModelType);
                return Build(Data);
            }
        }

        private TMODEL Build<TMODEL>(TMODEL Data)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                Data.FeedbackAPI = FeedbackAPI;
                Data.AuthenticationAPI = AuthenticationAPI;
                Data.ModelAPI = ModelAPI;
                Data.BuildDenied(AuthenticationAPI.AuthenticatedId);

                return Data;
            }
        }

        private TRETURN Check<TMODEL>(TMODEL DataModel, Action RequestedAction)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                if ((RequestedAction == Action.Read && DataModel.ReadAccess == Access.Authenticated ||
                     RequestedAction == Action.Write && DataModel.WriteAccess == Access.Authenticated) &&
                     !AuthenticationAPI.Authenticated)
                {
                    return AuthenticateResult();
                }

                if (RequestedAction == Action.Read && DataModel.ReadAccess == Access.Permissioned ||
                    RequestedAction == Action.Write && DataModel.WriteAccess == Access.Permissioned)
                {
                    if (!AuthenticationAPI.Authenticated)
                        return AuthenticateResult();

                    if (AuthenticationAPI.LoggedInCredentialState == "Expired") // Magic string to be fixed
                        return PasswordResetResult(DataModel);

                    var ActionName = RequestedAction.ToString();
                    var Name = DataModel.GetType().Name;
                    var Area = string.Join(string.Empty, DataModel.GetType().Namespace.Skip("Fido.Action.Models.".Length)); // to do: remove magic string

                    var Allowed = DataModel.Allowed(ActionName, Name, Area);

                    Log.InfoFormat("Permission: {0}, {1}, {2} = {3}", ActionName, Name, Area, Allowed);

                    if (!Allowed)
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
