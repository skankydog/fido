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
                RequiredPermission: Permission.Read);
        }

        public TRETURN View(Func<NoModel, TRETURN> Result)
        {
            return DoView<NoModel>(
                DataModel: new NoModel(),
                SuccessResult: Result,
                RequiredPermission: Permission.Read);
        }

        public TRETURN Create<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result)
            where TMODEL : IModel<TMODEL>
        {
            return DoView(
                DataModel: DataModel,
                SuccessResult: Result,
                RequiredPermission: Permission.Write);
        }

        private TRETURN DoView<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Permission RequiredPermission)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                var Model = Build(DataModel);
                var Redirect = CheckPermissions(Model, RequiredPermission);

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
                var Redirect = CheckPermissions(Model, Permission.Read);

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
                var Redirect = CheckPermissions(Model, Permission.Read);

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
                var Redirect = CheckPermissions(Model, Permission.Write);

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
                var Redirect = CheckPermissions(Model, Permission.Write);

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
                //var ModelPath = string.Concat(SourceAssembly.GetName().Name, ".Models.", typeof(TMODEL).Name);
                var ModelPath = typeof(TMODEL).FullName;
                //var x = typeof(TMODEL).Namespace;
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
                Guid Id = AuthenticationAPI.AuthenticatedId;
                Data.Permissions = new List<Dtos.Activity>();

                if (Id != Guid.Empty)
                {
                    var UserService = ServiceFactory.CreateService<IUserService>();
                    var Activities = UserService.GetActivities(Id);

                    Data.Permissions = (from Dtos.Activity a in Activities
                                        select a).ToList();
                }

                Data.FeedbackAPI = FeedbackAPI;
                Data.AuthenticationAPI = AuthenticationAPI;
                Data.ModelAPI = ModelAPI;

                return Data;
            }
        }

        private TRETURN CheckPermissions<TMODEL>(TMODEL DataModel, Permission RequestedPermission)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                if (DataModel.RequiresReadPermission && RequestedPermission == Permission.Read ||
                    DataModel.RequiresWritePermission && RequestedPermission == Permission.Write)
                {
                    if (!AuthenticationAPI.Authenticated)
                        return AuthenticateResult();

                    if (AuthenticationAPI.LoggedInCredentialState == "Expired") // Magic string to be fixed
                        return PasswordResetResult(DataModel);

                    var Name = DataModel.GetType().Name;
                    var Area = string.Join(string.Empty, DataModel.GetType().Namespace.Skip("Fido.Action.Models.".Length)); // to do: remove magic string
                    var Action = RequestedPermission.ToString();
                    var Allowed = (from Dtos.Activity a in DataModel.Permissions
                                   where a.Name == Name &&
                                        a.Area == Area && 
                                        a.Action == Action
                                   select a).Count() == 1;

                    Log.InfoFormat("Permission: {0}, {1}, {2} = {3}", Name, Area, Action, Allowed);

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
