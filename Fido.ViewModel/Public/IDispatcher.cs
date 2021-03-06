﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Fido.Core;
using Fido.ViewModel.Implementation;

namespace Fido.ViewModel
{
    public interface IDispatcher<TRETURN> where TRETURN : class
    {
        TRETURN Index<TMODEL>(Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Index<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN List<TMODEL>(ListOptions IndexOptions, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;

        TRETURN Load<TMODEL>(Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Load<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;

        TRETURN Create<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Create<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult) where TMODEL : IModel<TMODEL>;

        TRETURN Update<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
        TRETURN Update<TMODEL>(
            TMODEL DataModel,
            Func<TMODEL, TRETURN> SuccessResult,
            Func<TMODEL, TRETURN> InvalidResult) where TMODEL : IModel<TMODEL>;

        TRETURN DeleteIt<TMODEL>(TMODEL DataModel, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;

        TRETURN Confirm<TMODEL>(Guid Id, Func<TMODEL, TRETURN> Result) where TMODEL : IModel<TMODEL>;
    }
}
