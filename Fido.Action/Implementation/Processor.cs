using System;
using Fido.Core;

namespace Fido.Action.Implementation
{
    internal class Processor<TMODEL, TRETURN>
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IFeedbackAPI FeedbackAPI { get; set; }
        protected IAuthenticationAPI AuthenticationAPI { get; set; }
        protected IModelAPI ModelAPI { get; set; }
        IModel<TMODEL> ModelLogic { get; set; }

        internal Processor(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI AuthenticationAPI,
            IModelAPI ModelAPI,
            IModel<TMODEL> ModelLogic)
        {
            this.FeedbackAPI = FeedbackAPI;
            this.AuthenticationAPI = AuthenticationAPI;
            this.ModelAPI = ModelAPI;
            this.ModelLogic = ModelLogic;
        }

        public TRETURN ExecuteRead(Guid Id, int? Page, Func<TMODEL, TRETURN> SuccessUI)
        {
            using (new FunctionLogger(Log))
            {
                TMODEL Model = default(TMODEL);

                try
                {
                    if (Page == null)
                    {
                        Model = ModelLogic.Read(Id);
                    }
                    else
                    {
                        Model = ModelLogic.Read(Id, (int)Page);
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
                        if (ModelLogic.Write(Model) == true)
                        {
                            Log.Info("Success returned from OnValidModel");
                            if (Model is IModelUI) ((IModelUI)Model).InputState = "Success";
                            return SuccessUI(Model);
                        }
                    }
                    catch (Exception Ex)
                    {
                        FeedbackAPI.DisplayError(Ex.Message);
                        Log.Info("Exception from OnValidModel: " + Ex.ToString());
                    }

                    ModelLogic.OnFailedWrite(Model);
                    if (Model is IModelUI) ((IModelUI)Model).InputState = "Failure";
                    return FailureUI(Model);
                }

                ModelLogic.OnInvalidWrite(Model);
                if (Model is IModelUI) ((IModelUI)Model).InputState = "Invalid";
                return InvalidUI(Model);
            }
        }
    }
}
