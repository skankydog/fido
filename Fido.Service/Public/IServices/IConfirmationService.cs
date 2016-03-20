using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Dtos;

namespace Fido.Service
{
    public interface IConfirmationService
    {
        IList<Confirmation> GetConfirmationsForUser(Guid UserId);
        IList<Confirmation> GetAllQueuedConfirmations();
        void MarkConfirmationAsSent(Guid Id);
        string GetConfirmationType(Guid Id);
    }
}
