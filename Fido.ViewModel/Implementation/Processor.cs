using System;
using Fido.Core;
using Fido.ViewModel.Models;

namespace Fido.ViewModel.Implementation
{
    internal class Processor<TRETURN>
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region View
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

                    DataModel.FeedbackAPI.DisplayError(Ex.Message);
                    return ErrorResult((IDataModel)DataModel);
                }

                return SuccessResult(DataModel);
            }
        }
        #endregion

        #region Read
        public TRETURN ExecuteRead<TMODEL>(ListOptions IndexOptions, TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            return DoExecuteRead(IndexOptions.Id, DataModel, IndexOptions, SuccessResult, ErrorResult);
        }

        public TRETURN ExecuteRead<TMODEL>(Guid Id, TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            return DoExecuteRead(Id, DataModel, null, SuccessResult, ErrorResult);
        }

        public TRETURN ExecuteRead<TMODEL>(Guid Id, ListOptions IndexOptions, TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            return DoExecuteRead(Id, DataModel, IndexOptions, SuccessResult, ErrorResult);
        }

        private TRETURN DoExecuteRead<TMODEL>(Guid Id, TMODEL DataModel, ListOptions IndexOptions, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                try
                {
                    var Tmp = DataModel.Denied; // Mapping wipes this out

                    if (IndexOptions == null)
                    {
                        DataModel = DataModel.Read(Id);
                    }
                    else
                    {
                        if (Id == Guid.Empty)
                            DataModel = DataModel.Read(IndexOptions);
                        else
                            DataModel = DataModel.Read(Id, IndexOptions);
                    }

                    if (DataModel != null) DataModel.Denied = Tmp; // Restore after mapping
                    if (DataModel != null) DataModel = DataModel.Prepare(DataModel);
                }
                catch (Exception Ex)
                {
                    Log.Fatal(Ex.ToString());
                    
                    DataModel.FeedbackAPI.DisplayError(Ex.Message);
                    return ErrorResult((IDataModel)DataModel);
                }

                return SuccessResult(DataModel);
            }
        }
        #endregion

        #region Write
        public TRETURN ExecuteWrite<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<TMODEL, TRETURN> InvalidResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                DataModel = DataModel.Prepare(DataModel);

                if (DataModel.ModelAPI.ModelStateIsValid())
                {
                    try
                    {
                        if (DataModel.Write(DataModel) == true)
                        {
                            Log.Info("Successful write");

                            DataModel.IsNew = false; // ensure caller now knows this is in the db
                            return SuccessResult(DataModel);
                        }
                    }
                    catch (Exception Ex)
                    {
                        Log.Fatal(Ex.ToString());
                        DataModel.FeedbackAPI.DisplayError(Ex.Message);
                    }

                    DataModel.OnFailedWrite(DataModel);
                    return ErrorResult((IDataModel)DataModel);
                }

                Log.Warn("Model is invalid");
                DataModel.OnInvalidWrite(DataModel);
                return InvalidResult(DataModel);
            }
        }
        #endregion

        #region Delete
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
                    DataModel.FeedbackAPI.DisplayError(Ex.Message);

                    return ErrorResult((IDataModel)DataModel);
                }

                return SuccessResult(DataModel);
            }
        }
        #endregion

        #region Confirm
        public TRETURN ExecuteConfirm<TMODEL>(Guid ConfirmationId, TMODEL DataModel, Func<TMODEL, TRETURN> SuccessResult, Func<IDataModel, TRETURN> ErrorResult)
            where TMODEL : IModel<TMODEL>
        {
            using (new FunctionLogger(Log))
            {
                try
                {
         // Don't believe this is affected for confirmation, only read as read returns a loaded model.
         //           var Tmp = DataModel.DeniedActivities; // Mapping wipes this out

                    if (DataModel.Confirm(ConfirmationId) == false)
                        Log.InfoFormat("Unsuccessful confirmation: {0}", ConfirmationId.ToString());

         //           DataModel.DeniedActivities = Tmp; // Restore after mapping
                    DataModel = DataModel.Prepare(DataModel);
                }
                catch (Exception Ex)
                {
                    Log.ErrorFormat("Exception thrown in confirmation: {0}", Ex.ToString());

                    DataModel.FeedbackAPI.DisplayError(Ex.Message);
                    return ErrorResult((IDataModel)DataModel);
                }

                return SuccessResult(DataModel);
            }
        }
        #endregion
    }
}
