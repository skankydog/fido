﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Entities;

namespace Fido.DataAccess
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User GetByExternalCredentials(string LoginProvider, string ProviderKey, string IncludeProperties = null);
        User GetByExternalEmailAddress(string EmailAddress, string IncludeProperties = null);
    }
}
