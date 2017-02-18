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

        #region Simple
        public TRETURN Simple(Func<NoModel, TRETURN> Result)
        {
            return View<NoModel>(Result, Function.Read);
        }
        #endregion

        #region Index
        public TRETURN Index<TMODEL>(Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            return View<TMODEL>(
                Result: Result,
                Function: Function.Read);
        }

        public TRETURN Index<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            var Model = BuildModel<TMODEL>();
            var Redirect = CheckPermissions(Model, Function.Read);

            if (Redirect != null)
                return Redirect;

            var Processor = new Processor<TRETURN>();
            return Processor.ExecuteRead<TMODEL>(
                Id: Id,
                DataModel: Model,
                SuccessResult: Result,
                ErrorResult: ErrorResult);
        }

        public TRETURN List<TMODEL>(ListOptions IndexOptions, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = BuildModel<TMODEL>();
                var Redirect = CheckPermissions(Model, Function.Read);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TRETURN>();
                return Processor.ExecuteRead<TMODEL>(
                    DataModel: Model,
                    IndexOptions: IndexOptions,
                    SuccessResult: Result,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Load
        public TRETURN Load<TMODEL>(Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            return View<TMODEL>(Result, Function.Write);
        }

        public TRETURN Load<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = BuildModel<TMODEL>();
                var Redirect = CheckPermissions(Model, Function.Read);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TRETURN>();
                return Processor.ExecuteRead(
                    Id: Id,
                    DataModel: Model,
                    SuccessResult: Result,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Create
        public TRETURN Create<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>
        {
            return Save(DataModel, Result);
        }

        public TRETURN Create<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult) where TMODEL : IModel<TMODEL>
        {
            return Save(DataModel, SuccessResult, InvalidResult);
        }
        #endregion

        #region Update
        public TRETURN Update<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>
        {
            return Save(DataModel, Result);
        }

        public TRETURN Update<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult) where TMODEL : IModel<TMODEL>
        {
            return Save(DataModel, SuccessResult, InvalidResult);
        }
        #endregion

        #region Delete
        public TRETURN DeleteIt<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = BuildModel(DataModel);
                var Redirect = CheckPermissions(Model, Function.Write);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TRETURN>();
                return Processor.ExecuteDelete(
                    DataModel: Model,
                    SuccessResult: SuccessResult,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Confirm
        public TRETURN Confirm<TMODEL>(Guid ConfirmationId, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = BuildModel<TMODEL>();

                var Processor = new Processor<TRETURN>();
                return Processor.ExecuteConfirm<TMODEL>(
                    ConfirmationId: ConfirmationId,
                    DataModel: Model,
                    SuccessResult: Result,
                    ErrorResult: ErrorResult);
            }
        }
        #endregion

        #region Private Functions
        //private TRETURN Read<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result, Function Function)
        //    where TMODEL : IModel<TMODEL>
        //{ // here!!
        //    using (new FunctionLogger(Log))
        //    {
        //        var Model = BuildModel<TMODEL>();
        //        var Redirect = CheckPermissions(Model, Function);

        //        if (Redirect != null)
        //            return Redirect;

        //        var Processor = new Processor<TRETURN>();
        //        return Processor.ExecuteRead(
        //            Id: Id,
        //            DataModel: Model,
        //            SuccessResult: Result,
        //            ErrorResult: ErrorResult);
        //    }
        //}

        private TRETURN View<TMODEL>(Func<TMODEL, TRETURN> Result, Function Function)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = BuildModel<TMODEL>();
                var Redirect = CheckPermissions(Model, Function);

                if (Redirect != null)
                    return Redirect;

                var Processor = new Processor<TRETURN>();
                return Processor.ExecuteView(
                    DataModel: Model,
                    SuccessResult: Result,
                    ErrorResult: ErrorResult);
            }
        }

        private TRETURN Save<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            return Save(
                DataModel: DataModel,
                SuccessResult: Result,
                InvalidResult: Result);
        }

        private TRETURN Save<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult)
                where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = BuildModel(DataModel);
                var Redirect = CheckPermissions(Model, Function.Write);

                if (Redirect != null)
                    return Redirect;

                var AuthenticatedBefore = AuthenticationAPI.Authenticated;
                
                var Result = new Processor<TRETURN>().ExecuteWrite(
                    DataModel: Model,
                    SuccessResult: SuccessResult,
                    InvalidResult: InvalidResult,
                    ErrorResult: ErrorResult);
                
                var AuthenticatedAfter = AuthenticationAPI.Authenticated;

                if (AuthenticatedBefore != AuthenticatedAfter)
                    Model = BuildModel(Model);

                return Result;
            }
        }

        private TMODEL BuildModel<TMODEL>()
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
                return BuildModel(Data);
            }
        }

        private TMODEL BuildModel<TMODEL>(TMODEL Data)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                Data.FeedbackAPI = FeedbackAPI;
                Data.AuthenticationAPI = AuthenticationAPI;
                Data.ModelAPI = ModelAPI;
                Data.BuildDeniedActivities(AuthenticationAPI.AuthenticatedId);

                return Data;
            }
        }

        private TRETURN CheckPermissions<TMODEL>(TMODEL DataModel, Function Function)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                if (AuthenticationAPI.LoggedInCredentialState == "Expired") // TO DO: Magic string to be fixed
                    return PasswordResetResult(DataModel);

                if (Function == Function.Read && DataModel.ReadAccess != Access.Anonymous ||
                    Function == Function.Write && DataModel.WriteAccess != Access.Anonymous)
                {
                    if (!AuthenticationAPI.Authenticated)
                        return AuthenticateResult();

                    var FunctionName = Function.ToString();
                    var ModelName = DataModel.GetType().Name;
                    var Namespace = string.Join(string.Empty, DataModel.GetType().Namespace.Skip("Fido.Action.Models.".Length)); // to do: remove magic string

                    if (!DataModel.Allowed(FunctionName, ModelName, Namespace))
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
