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
        TRETURN View<TMODEL>(Func<TRETURN> Result);
        TRETURN Read<TMODEL>(Guid Id, IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result);
        TRETURN Read<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result);
        TRETURN Write<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> FailureResult,
            Func<TMODEL, TRETURN> InvalidResult);
        TRETURN Write<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> AnyResult);
        TRETURN Delete_<TMODEL>(TMODEL DataModel, Func<TRETURN> Result);
    }
}
