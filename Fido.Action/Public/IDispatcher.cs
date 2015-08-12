using System;
using System.Collections.Generic;
using System.Reflection;
using Fido.Core;
using Fido.Action.Implementation;

namespace Fido.Action
{
    public interface IDispatcher<TRETURN>
    {
        TRETURN View<TMODEL>(Func<TRETURN> UI);
        TRETURN Read<TMODEL>(Guid Id, int Page, Func<TMODEL, TRETURN> SuccessUI);
        TRETURN Read<TMODEL>(Guid Id, Func<TMODEL, TRETURN> SuccessUI);
        TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> SuccessUI,
            Func<TMODEL, TRETURN> FailureUI,
            Func<TMODEL, TRETURN> InvalidUI);
        TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> UI);
    }
}
