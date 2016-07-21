﻿using System;
using System.Collections.Generic;
using Fido.Action.Models;

namespace Fido.Action.Implementation
{
    public interface ILogicModel
    {
        IFeedbackAPI FeedbackAPI { set; }
        IAuthenticationAPI AuthenticationAPI { set; }
        IModelAPI ModelAPI { set; }

        Access ReadAccess { get; }
        Access WriteAccess { get; }
    }

    public interface IDataModel
    {
        IList<Dtos.Activity> Permissions { get; set; }

        //bool HasWritePermission(string Name, string Area);
        //bool HasReadPermission(string Name, string Area);
        //bool HasReadOrWritePermission(string Name, string Area);
        //bool HasWritePermissions(string Area);
        //bool HasReadPermissions(string Area);
        bool HasArea(string Area);
    }

    public interface IModel<TMODEL> : ILogicModel, IDataModel
        where TMODEL : IModel<TMODEL>
    {
        TMODEL Prepare(TMODEL Model);
        TMODEL Read(Guid Id);
        TMODEL Read(IndexOptions IndexOptions);
        bool Save(TMODEL Model);
        bool Confirm(Guid ConfirmationId);
        bool Delete(TMODEL Model);
        void OnInvalidWrite(TMODEL Model);
        void OnFailedWrite(TMODEL Model);
    }
}
