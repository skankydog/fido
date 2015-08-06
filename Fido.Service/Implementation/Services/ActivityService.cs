using AutoMapper;
//using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Core;
using Fido.Dtos;
using Fido.DataAccess;

namespace Fido.Service.Implementation
{
    internal class ActivityService : CRUDService<Dtos.Activity, Entities.Activity, IActivityRepository>, IActivityService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Activity GetByName(string Name)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IActivityRepository>(UnitOfWork);
                    var ActivityEntity = Repository.Get(e => e.Name == Name);

                    var ActivityDTO = Mapper.Map<Entities.Activity, Activity>(ActivityEntity);

                    return ActivityDTO;
                }
            }
        }

        public bool NameFree(string Name)
        {
            using (new FunctionLogger(Log))
            {
                if (GetByName(Name) == null)
                    return true;

                return false;
            }
        }
    }
}
