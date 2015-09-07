using System;
using System.Collections.Generic;
using System.Reflection;
using Fido.Core;
using Fido.Action.Implementation;

namespace Fido.Action
{
    public interface IDispatcher<TRETURN>
    {
        TRETURN View<TMODEL>(Func<TRETURN> Any);
        TRETURN Read<TMODEL>(Guid Id, int Page, Func<TMODEL, TRETURN> Success);
        TRETURN Read<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Success);
        TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> Success,
            Func<TMODEL, TRETURN> Failure,
            Func<TMODEL, TRETURN> Invalid);
        TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> Any);
        TRETURN Delete_<TMODEL>(Guid Id, Func<TRETURN> Any);
    }
}
