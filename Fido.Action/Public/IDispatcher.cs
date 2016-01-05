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
        TRETURN IndexView<TMODEL>(IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result);
        TRETURN CreateView<TMODEL>(Func<TRETURN> Result); // Not sure I should allow for parameterless delegates
        TRETURN CreateView<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result);
        TRETURN UpdateView<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result);
        TRETURN DeleteView<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result);

        TRETURN Create<TMODEL>(
            TMODEL DataModel,
            Func<TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> FailureResult,
            Func<TMODEL, TRETURN> InvalidResult);
        TRETURN Create<TMODEL>(
            TMODEL DataModel,
            Func<TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> NonsuccessResult);

        TRETURN Update<TMODEL>(
            TMODEL DataModel,
            Func<TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> FailureResult,
            Func<TMODEL, TRETURN> InvalidResult);
        TRETURN Update<TMODEL>(
            TMODEL DataModel,
            Func<TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> NonsuccessResult);

        TRETURN Delete_<TMODEL>(TMODEL DataModel, Func<TRETURN> Result);
    }
}
