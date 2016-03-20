using System;
using Fido.Core;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    internal class Processor<TMODEL, TRETURN>
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IFeedbackAPI FeedbackAPI { get; set; }
        protected IAuthenticationAPI AuthenticationAPI { get; set; }
        protected IModelAPI ModelAPI { get; set; }
        IModel<TMODEL> LogicModel { get; set; }

        internal Processor(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI AuthenticationAPI,
            IModelAPI ModelAPI,
            IModel<TMODEL> LogicModel)
        {
            this.FeedbackAPI = FeedbackAPI;
            this.AuthenticationAPI = AuthenticationAPI;
            this.ModelAPI = ModelAPI;
            this.LogicModel = LogicModel;
        }

        public TRETURN ExecuteView(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            using (new FunctionLogger(Log))
            {
                try
                {
                    DataModel = LogicModel.Prepare(DataModel);
                }
                catch (Exception Ex)
                {
                    Log.Fatal(Ex.ToString());

                    FeedbackAPI.DisplayError(Ex.Message);
                    return ErrorResult();
                }

                return SuccessResult(DataModel);
            }
        }

        #region Index Reads
        //public TRETURN ExecuteRead(IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result)
        //{
        //    return DoExecuteRead(Guid.Empty, IndexOptions, Result, Result);
        //}

        public TRETURN ExecuteRead(IndexOptions IndexOptions, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            return DoExecuteRead(Guid.Empty, IndexOptions, SuccessResult, ErrorResult);
        }
        #endregion

        #region Non-Index Reads
        //public TRETURN ExecuteRead(Guid Id, Func<TMODEL, TRETURN> Result)
        //{
        //    return DoExecuteRead(Id, null, Result, Result);
        //}

        public TRETURN ExecuteRead(Guid Id, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            return DoExecuteRead(Id, null, SuccessResult, ErrorResult);
        }
        #endregion

        private TRETURN DoExecuteRead(Guid Id, IndexOptions IndexOptions, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            using (new FunctionLogger(Log))
            {
                TMODEL DataModel = default(TMODEL);

                try
                {
                    if (IndexOptions == null)
                    {
                        DataModel = LogicModel.Read(Id);
                    }
                    else
                    {
                        DataModel = LogicModel.Read(IndexOptions);
                    }

                    DataModel = LogicModel.Prepare(DataModel);
                }
                catch (Exception Ex)
                {
                    Log.Fatal(Ex.ToString());
                    
                    FeedbackAPI.DisplayError(Ex.Message);
                    return ErrorResult();
                }

                return SuccessResult(DataModel);
            }
        }

        public TRETURN ExecuteWrite(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<TMODEL, TRETURN> InvalidResult, Func<TRETURN> ErrorResult)
        {
            using (new FunctionLogger(Log))
            {
                DataModel = LogicModel.Prepare(DataModel);

                if (ModelAPI.ModelStateIsValid())
                {
                    try
                    {
                        if (LogicModel.Save(DataModel) == true)
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

                    LogicModel.OnFailedWrite(DataModel);
                    return ErrorResult();
                }

                Log.Warn("Model is invalid");
                LogicModel.OnInvalidWrite(DataModel);
                return InvalidResult(DataModel);
            }
        }

        public TRETURN ExecuteDelete(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<TRETURN> ErrorResult)
        {
            using (new FunctionLogger(Log))
            {
                try
                {
                    LogicModel.Delete(DataModel);
                }
                catch (Exception Ex)
                {
                    Log.Fatal(Ex.ToString());
                    FeedbackAPI.DisplayError(Ex.Message);

                    return ErrorResult();
                }

                return SuccessResult(DataModel);
            }
        }
    }
}
