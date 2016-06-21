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
        IList<Activity> GetPageInDefaultOrder(char SortOrder, int Skip, int Take, string Filter);
        IList<Activity> GetPageInNameOrder(char SortOrder, int Skip, int Take, string Filter);
        IList<Activity> GetPageInAreaOrder(char SortOrder, int Skip, int Take, string Filter);
        IList<Activity> GetPageInActionOrder(char SortOrder, int Skip, int Take, string Filter);

        Activity Get(string Name, string Area, string Action);
        Activity GetByName(string Name);                                     //  TO DO: Doubt this is needed
        bool NameFree(string Name);             // Name must be unique           TO DO: Doubt this is needed
    }
}
