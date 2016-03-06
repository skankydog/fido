//using System;
//using System.Collections.Generic;
//using Fido.Core;
//using Fido.Service;
//using Fido.Action.Implementation;

//namespace Fido.Action.Models
//{
//    public class UnlinkExternalCredentialsVM : Model<UnlinkExternalCredentialsVM>
//    {
//        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

//        #region Data
//        public Guid Id { get; set; }
////        public DateTime CreatedUtc { get; set; }
////        public bool IsNew { get; set; }
////        public byte[] RowVersion { get; set; }
//        #endregion

//        public UnlinkExternalCredentialsVM()
//        {
//            Id = Guid.NewGuid();
////            CreatedUtc = DateTime.UtcNow;
////            IsNew = true;
//        }

//        public UnlinkExternalCredentialsVM(
//            IFeedbackAPI FeedbackAPI,
//            IAuthenticationAPI LoginAPI,
//            IModelAPI ModelAPI)
//                : base (FeedbackAPI, LoginAPI, ModelAPI,
//                        RequiresReadPermission: true, RequiresWritePermission: true)
//        { }

//        public override bool Save(UnlinkExternalCredentialsVM Model)
//        {
//            using (new FunctionLogger(Log))
//            {
//                var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

//                AuthenticationService.UnlinkExternalCredentials(
//                    AuthenticationAPI.AuthenticatedId,
//                    Model.Id);

//                FeedbackAPI.DisplaySuccess("The external credentials have been removed");
//                return true;
//            }
//        }
//    }
//}
