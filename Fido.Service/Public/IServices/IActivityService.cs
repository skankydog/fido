using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Dtos;

namespace Fido.Service
{
    public interface IActivityService : ICRUDService<Activity>
    {
        Activity GetByName(string Name);
        bool NameFree(string Name);             // Name must be unique
    }
}
