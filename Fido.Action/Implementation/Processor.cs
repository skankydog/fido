using System;
using Fido.Core;

namespace Fido.Action.Implementation
{
    internal class Processor<TMODEL, TRETURN>
        where TMODEL : IModel
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IFeedbackAPI FeedbackAPI { get; set; }
        protected IAuthenticationAPI AuthenticationAPI { get; set; }
        protected IModelAPI ModelAPI { get; set; }
        IHandler<TMODEL> ModelHandler { get; set; }

        internal Processor(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI AuthenticationAPI,
            IModelAPI ModelAPI,
            IHandler<TMODEL> ModelHandler)
        {
            this.FeedbackAPI = FeedbackAPI;
            this.AuthenticationAPI = AuthenticationAPI;
            this.ModelAPI = ModelAPI;
            this.ModelHandler = ModelHandler;
        }

        public TRETURN ExecuteRead(Guid Id, Func<TMODEL, TRETURN> SuccessUI, int? Page)
        {
            using (new FunctionLogger(Log))
            {
                TMODEL Model = default(TMODEL);

                try
                {
                    if (Page == null)
                    {
                        Model = ModelHandler.Read(Id);
                    }
                    else
                    {
                        Model = ModelHandler.Read(Id, (int)Page);
                    }
                }
                catch (Exception Ex)
                {
                    FeedbackAPI.DisplayError(Ex.Message);
                    Log.Fatal(Ex.ToString());
                }

                return SuccessUI(Model);
            }
        }

        public TRETURN ExecuteWrite(TMODEL Model, Func<TMODEL, TRETURN> SuccessUI, Func<TMODEL, TRETURN> FailureUI, Func<TMODEL, TRETURN> InvalidUI)
        {
            using (new FunctionLogger(Log))
            {
                if (ModelAPI.ModelStateIsValid())
                {
                    try
                    {
                        if (ModelHandler.Write(Model) == true)
                        {
                            Log.Info("Success returned from OnValidModel");
                            Model.FormState = "Success";
                            return SuccessUI(Model);
                        }
                    }
                    catch (Exception Ex)
                    {
                        FeedbackAPI.DisplayError(Ex.Message);
                        Log.Info("Exception from OnValidModel: " + Ex.ToString());
                    }

                    ModelHandler.OnFailedWrite(Model);
                    Model.FormState = "Failure";
                    return FailureUI(Model);
                }

                ModelHandler.OnInvalidWrite(Model);
                Model.FormState = "Invalid";
                return InvalidUI(Model);
            }
        }
    }
}
