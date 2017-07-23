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
    internal class ConfirmationService : CRUDService<Confirmation, Entities.Confirmation, DataAccess.IConfirmationRepository>, IConfirmationService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Pages
        public IList<Confirmation> GetPageInDefaultOrder(Guid Id, char SortOrder, int Skip, int Take, string Filter)
        {
            return GetPage(Id, SortOrder, Skip, Take, Filter,
                OrderByAscending: q => q.OrderBy(s => s.Id),
                OrderByDescending: q => q.OrderByDescending(s => s.Id));
        }

        private IList<Confirmation> GetPage(Guid Id, char SortOrder, int Skip, int Take, string Filter,
            Func<IQueryable<Entities.Confirmation>, IOrderedQueryable<Entities.Confirmation>> OrderByAscending,
            Func<IQueryable<Entities.Confirmation>, IOrderedQueryable<Entities.Confirmation>> OrderByDescending)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var ConfirmationRepository = DataAccessFactory.CreateRepository<IConfirmationRepository>(UnitOfWork);
                    var OrderBy = SortOrder == 'a' ? OrderByAscending : OrderByDescending;
                    var Query = ConfirmationRepository.GetAsIQueryable(e => e.Id != null && e.UserId == Id, OrderBy);

                    if (Filter.IsNotNullOrEmpty())
                    {
                        Query = Query.Where(e => e.ConfirmType.ToLower().Contains(Filter.ToLower())
                                              || e.EmailAddress.ToLower().Contains(Filter.ToLower()));
                                              // State is a property & not stored in the db, can't be filtered
                    }

                    Query = Query.Skip(Skip).Take(Take);

                    var EntityList = Query.ToList(); // Hit the database

                    IList<Confirmation> DtoList = AutoMapper.Mapper.Map<IList<Entities.Confirmation>, IList<Confirmation>>(EntityList);
                    return DtoList;
                }
            }
        }
        #endregion

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

        #region Hooks
        protected override void BeforeDelete(Guid Id, IUnitOfWork UnitOfWork)
        {
            var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfirmationRepository>(UnitOfWork);
            var ConfirmationEntity = Repository.Get(Id);

            if (!ConfirmationEntity.Deletable)
                throw new Exception("The confirmation is not in a state that allows it to be deleted");
        }
        #endregion

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
                var Queued = Repository.Get(e => e.Id == Id && e.ConfirmType == Type && e.QueuedUTC != null /*&& e.SentUTC != null*/ && e.ReceivedUTC == null);

                if (Queued == null)
                {
                    throw new ServiceException(string.Format("Failed to retrieve confirmation: Id={0}, Type={1}, Queued=<not null>, Received=<not null>", Id, Type));
                }

                Queued.ReceivedUTC = DateTime.UtcNow;

                Log.InfoFormat("Received confirmation. Updating: Id={0}, Type={1}, Queued={2}(UTC), Sent={3}(UTC), Received={4}(UTC)",
                    Queued.Id, Queued.ConfirmType, Queued.CreatedUtc, Queued.SentUTC, Queued.ReceivedUTC);
                
                Repository.Update(Queued);
                return Queued;
            }
        }
        #endregion
    }
}
