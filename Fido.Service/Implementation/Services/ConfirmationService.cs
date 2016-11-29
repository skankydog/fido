using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Service.Exceptions;
using Fido.Core;
using Fido.Dtos;
using Fido.DataAccess;

namespace Fido.Service.Implementation
{
    internal class ConfirmationService : IConfirmationService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IList<Confirmation> GetAll(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfirmationRepository>(UnitOfWork);
                    var ConfirmationEntities = Repository.GetAsIEnumerable(e => e.UserId == UserId).ToList();

                    IList<Confirmation> ConfirmationDTOs = null;
                    ConfirmationDTOs = Mapper.Map<IList<Entities.Confirmation>, IList<Confirmation>>(ConfirmationEntities, ConfirmationDTOs);

                    return ConfirmationDTOs;
                }
            }
        }

        public IList<Confirmation> GetQueued(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfirmationRepository>(UnitOfWork);
                    var ConfirmationEntities = Repository.GetAsIEnumerable(e => e.UserId == UserId && e.QueuedUTC != null && e.SentUTC == null).ToList();

                    IList<Confirmation> ConfirmationDTOs = null;
                    ConfirmationDTOs = Mapper.Map<IList<Entities.Confirmation>, IList<Confirmation>>(ConfirmationEntities, ConfirmationDTOs);

                    return ConfirmationDTOs;
                }
            }
        }

        public IList<Confirmation> GetSent(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfirmationRepository>(UnitOfWork);
                    var ConfirmationEntities = Repository.GetAsIEnumerable(e => e.UserId == UserId && e.SentUTC != null && e.ReceivedUTC == null).ToList();

                    IList<Confirmation> ConfirmationDTOs = null;
                    ConfirmationDTOs = Mapper.Map<IList<Entities.Confirmation>, IList<Confirmation>>(ConfirmationEntities, ConfirmationDTOs);

                    return ConfirmationDTOs;
                }
            }
        }

        public IList<Confirmation> GetReceived(Guid UserId)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfirmationRepository>(UnitOfWork);
                    var ConfirmationEntities = Repository.GetAsIEnumerable(e => e.UserId == UserId && e.ReceivedUTC != null).ToList();

                    IList<Confirmation> ConfirmationDTOs = null;
                    ConfirmationDTOs = Mapper.Map<IList<Entities.Confirmation>, IList<Confirmation>>(ConfirmationEntities, ConfirmationDTOs);

                    return ConfirmationDTOs;
                }
            }
        }

        public IList<Confirmation> GetQueued()
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfirmationRepository>(UnitOfWork);
                    var ConfirmationEntities = Repository.GetAsIEnumerable(e => e.QueuedUTC != null && e.SentUTC == null).ToList();

                    IList<Confirmation> ConfirmationDTOs = null;
                    ConfirmationDTOs = Mapper.Map<IList<Entities.Confirmation>, IList<Confirmation>>(ConfirmationEntities, ConfirmationDTOs);

                    return ConfirmationDTOs;
                }
            }
        }

        public void MarkAsSent(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfirmationRepository>(UnitOfWork);
                    var Queued = Repository.Get(e => e.Id == Id && e.QueuedUTC != null && e.SentUTC == null && e.ReceivedUTC == null);

                    if (Queued == null)
                        throw new ServiceException("The validation is either non-existent, not ready to be sent or is in an invalid state");

                    Queued.SentUTC = DateTime.UtcNow;
                    Repository.Update(Queued);

                    UnitOfWork.Commit();
                }
            }
        }

        //public string GetConfirmationType(Guid Id)
        //{
        //    using (new FunctionLogger(Log))
        //    {
        //        using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
        //        {
        //            var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfirmationRepository>(UnitOfWork);
        //            var ConfirmationEntity = Repository.Get(e => e.Id == Id && e.QueuedUTC != null && e.SentUTC == null && e.ReceivedUTC == null);

        //            if (ConfirmationEntity == null)
        //                throw new ServiceException("The validation is either non-existent, not ready to be sent or is in an invalid state");

        //            return ConfirmationEntity.ConfirmType;
        //        }
        //    }
        //}

        #region Static Functions
        public static Guid QueueConfirmation(IUnitOfWork UnitOfWork, string Type, Guid UserId, string EmailAddress, bool AssumeSent = false)
        {
            using (new FunctionLogger(Log))
            {
                Log.DebugFormat("Type: {0}", Type);
                Log.DebugFormat("User Id: {0}", UserId.ToString());
                Log.DebugFormat("Email Address: {0}", EmailAddress);

                var Repository = DataAccessFactory.CreateRepository<IConfirmationRepository>(UnitOfWork);

                var Queued = new Entities.Confirmation
                {
                    Id = Guid.NewGuid(),
                    CreatedUtc = DateTime.UtcNow,
                    ConfirmType = Type,
                    UserId = UserId,
                    EmailAddress = EmailAddress,
                    QueuedUTC = DateTime.UtcNow,
                    SentUTC = (AssumeSent == true) ? DateTime.UtcNow : (DateTime?)null
                };

                Repository.CascadeInsert(Queued);
                return Queued.Id;
            }
        }

        public static Entities.Confirmation ReceiveConfirmation(IUnitOfWork UnitOfWork, Guid Id, string Type)
        {
            using (new FunctionLogger(Log))
            {
                var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfirmationRepository>(UnitOfWork);
                var Queued = Repository.Get(e => e.Id == Id && e.ConfirmType == Type && e.QueuedUTC != null && e.SentUTC != null && e.ReceivedUTC == null);

                if (Queued == null)
                    throw new ServiceException("The validation is either not ready to be received or is in an invalid state");

                Queued.ReceivedUTC = DateTime.UtcNow;
                Repository.Update(Queued);
                return Queued;
            }
        }
        #endregion
    }
}
