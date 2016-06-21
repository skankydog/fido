using System;
using Fido.Core;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    internal class Processor<TRETURN>
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IFeedbackAPI FeedbackAPI { get; set; }
        protected IAuthenticationAPI AuthenticationAPI { get; set; }
        protected IModelAPI ModelAPI { get; set; }

        internal Processor(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI AuthenticationAPI,
            IModelAPI ModelAPI)
        {
            this.FeedbackAPI = FeedbackAPI;
            this.AuthenticationAPI = AuthenticationAPI;
            this.ModelAPI = ModelAPI;
        }

        public TRETURN ExecuteView<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                try
                {
                    DataModel = DataModel.Prepare(DataModel);
                }
                catch (Exception Ex)
                {
                    Log.Fatal(Ex.ToString());

                    FeedbackAPI.DisplayError(Ex.Message);
                    return ErrorResult((IDataModel)DataModel);
                }

                return SuccessResult(DataModel);
            }
        }

        #region Index Reads
        public TRETURN ExecuteRead<TMODEL>(IndexOptions IndexOptions, TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            return DoExecuteRead(Guid.Empty, DataModel, IndexOptions, SuccessResult, ErrorResult);
        }
        #endregion

        #region Non-Index Reads
        public TRETURN ExecuteRead<TMODEL>(Guid Id, TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            return DoExecuteRead(Id, DataModel, null, SuccessResult, ErrorResult);
        }
        #endregion

        private TRETURN DoExecuteRead<TMODEL>(Guid Id, TMODEL DataModel, IndexOptions IndexOptions, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                try
                {
                    if (IndexOptions == null)
                    {
                        DataModel = DataModel.Read(Id);
                    }
                    else
                    {
                        DataModel = DataModel.Read(IndexOptions);
                    }

                    DataModel = DataModel.Prepare(DataModel);
                }
                catch (Exception Ex)
                {
                    Log.Fatal(Ex.ToString());
                    
                    FeedbackAPI.DisplayError(Ex.Message);
                    return ErrorResult((IDataModel)DataModel);
                }

                return SuccessResult(DataModel);
            }
        }

        public TRETURN ExecuteWrite<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<TMODEL, TRETURN> InvalidResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                DataModel = DataModel.Prepare(DataModel);

                if (ModelAPI.ModelStateIsValid())
                {
                    try
                    {
                        if (DataModel.Save(DataModel) == true)
                        {
                            Log.Info("Successful write");
                            return SuccessResult(DataModel);
                        }
                    }
                    catch (Exception Ex)
                    {
                        Log.Fatal(Ex.ToString());
                        FeedbackAPI.DisplayError(Ex.Message);
                    }

                    DataModel.OnFailedWrite(DataModel);
                    return ErrorResult((IDataModel)DataModel);
                }

                Log.Warn("Model is invalid");
                DataModel.OnInvalidWrite(DataModel);
                return InvalidResult(DataModel);
            }
        }

        public TRETURN ExecuteDelete<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                try
                {
                    DataModel.Delete(DataModel);
                }
                catch (Exception Ex)
                {
                    Log.Fatal(Ex.ToString());
                    FeedbackAPI.DisplayError(Ex.Message);

                    return ErrorResult((IDataModel)DataModel);
                }

                return SuccessResult(DataModel);
            }
        }
    }
}
