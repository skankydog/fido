using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Dtos;

namespace Fido.Service
{
    public interface ICRUDService<TDTO>
        where TDTO : Dto
    {
        TDTO Get(Guid Id, string IncludeProperties = "");
        int CountAll();
        IList<TDTO> GetAll();
        TDTO Save(TDTO DTO);
        bool Delete(Guid Id);
    }
}
