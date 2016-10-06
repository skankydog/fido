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
        TRETURN View(Func<NoModel, TRETURN> Result);

        TRETURN Index<TMODEL>(Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Index<TMODEL>(IndexOptions IndexOptions, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;

        TRETURN Create<TMODEL>(Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Create<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Create<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult) where TMODEL : IModel<TMODEL>;

        TRETURN Update<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Update<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Update<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult) where TMODEL : IModel<TMODEL>;

        //TRETURN Save<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        //TRETURN Save<TMODEL>(
        //    TMODEL DataModel,
        //    Func<TMODEL, TRETURN> SuccessResult,
        //    Func<TMODEL, TRETURN> InvalidResult) where TMODEL : IModel<TMODEL>;

        TRETURN Delete<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Delete<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;

        TRETURN Confirm<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
    }
}
