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

        public TRETURN ExecuteRead(Guid Id, IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result)
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
                        DataModel = LogicModel.Read(Id, IndexOptions);
                    }
                }
                catch (Exception Ex)
                {
                    FeedbackAPI.DisplayError(Ex.Message);
                    Log.Fatal(Ex.ToString());
                }

                return Result(DataModel);
            }
        }

        public TRETURN ExecuteWrite(TMODEL DataModel, Func<TRETURN> SuccessResult, Func<TMODEL, TRETURN> FailureResult, Func<TMODEL, TRETURN> InvalidResult)
        {
            using (new FunctionLogger(Log))
            {
                if (ModelAPI.ModelStateIsValid())
                {
                    try
                    {
                        if (LogicModel.Write(DataModel) == true)
                        {
                            Log.Info("Successful write");
                            if (DataModel is IModel) { ((IModel)DataModel).State = "Valid"; }
                            return SuccessResult();
                        }
                    }
                    catch (Exception Ex)
                    {
                        FeedbackAPI.DisplayError(Ex.Message);
                        Log.Info("Exception thrown in write: " + Ex.ToString());
                    }

                    LogicModel.OnFailedWrite(DataModel);
                    if (DataModel is IModel) { ((IModel)DataModel).State = "Failed"; }
                    return FailureResult(DataModel);
                }

                Log.Info("Invalid model");
                LogicModel.OnInvalidWrite(DataModel);
                if (DataModel is IModel) { ((IModel)DataModel).State = "Invalid"; }
                return InvalidResult(DataModel);
            }
        }

        public TRETURN ExecuteDelete(TMODEL DataModel, Func<TRETURN> Result)
        {
            using (new FunctionLogger(Log))
            {
                LogicModel.Delete(DataModel);
                return Result();
            }
        }
    }
}
