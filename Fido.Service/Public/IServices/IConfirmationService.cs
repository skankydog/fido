using System;
using System.Collections.Generic;
using Fido.Dtos;

namespace Fido.Service
{
    public interface IConfirmationService : ICRUDService<Confirmation>
    {
        IList<Confirmation> GetPageInDefaultOrder(Guid Id, char SortOrder, int Skip, int Take, string Filter);

        IList<Confirmation> GetAll(Guid UserId);
        IList<Confirmation> GetQueued(Guid UserId);
        IList<Confirmation> GetSent(Guid UserId);
        IList<Confirmation> GetReceived(Guid UserId);

        IList<Confirmation> GetQueued();

        void MarkAsSent(Guid Id);
    }
}
