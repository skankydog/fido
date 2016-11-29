using System;
using System.Collections.Generic;
using Fido.Dtos;

namespace Fido.Service
{
    public interface IConfirmationService
    {
        IList<Confirmation> GetAll(Guid UserId);
        IList<Confirmation> GetQueued(Guid UserId);
        IList<Confirmation> GetSent(Guid UserId);
        IList<Confirmation> GetReceived(Guid UserId);

        IList<Confirmation> GetQueued();

        void MarkAsSent(Guid Id);
    }
}
