using System;
using System.Collections.Generic;
using System.Reflection;
using Fido.Core;
using Fido.Action.Implementation;

namespace Fido.Action
{
    public interface IDispatcher<TRETURN>
    {
        TRETURN View<TMODEL>(Func<TRETURN> UI)
            where TMODEL : IModel;
        TRETURN Read<TMODEL>(Guid Id, int Page, Func<TMODEL, TRETURN> SuccessUI)
            where TMODEL : IModel;
        TRETURN Read<TMODEL>(Guid Id, Func<TMODEL, TRETURN> SuccessUI)
            where TMODEL : IModel;
        TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> SuccessUI,
            Func<TMODEL, TRETURN> FailureUI,
            Func<TMODEL, TRETURN> InvalidUI)
                where TMODEL : IModel;
        TRETURN Write<TMODEL>(
            TMODEL Model,
            Func<TMODEL, TRETURN> UI)
                where TMODEL : IModel;
    }
}
