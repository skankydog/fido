using System;
using System.Collections.Generic;
using System.Reflection;
using Fido.Core;
using Fido.Action.Implementation;
using Fido.Action.Models;

namespace Fido.Action
{
    public interface IDispatcher<TRETURN>
    {
        TRETURN ReturnIndexWrapper<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result);
        TRETURN ReturnLoadedModel<TMODEL>(IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result);
        TRETURN ReturnEmptyModel<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result);
        TRETURN ReturnLoadedModel<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result);
        TRETURN SavePostedModel<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> NonsuccessResult);
        TRETURN DeletePostedModel<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result);
    }
}
