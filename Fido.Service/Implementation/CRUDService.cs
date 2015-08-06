using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Service;
using Fido.Core;
using Fido.Core.Exceptions;
using Fido.Dtos;
using Fido.DataAccess;

namespace Fido.Service.Implementation
{
    internal abstract class CRUDService<TDTO, TENTITY, TIREPOSITORY> : ICRUDService<TDTO>
            where TDTO : Dto
            where TENTITY : Entities.Entity
            where TIREPOSITORY : DataAccess.IGenericRepository<TENTITY>
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TDTO Get(Guid Id, string IncludeProperties = "")
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    TIREPOSITORY Repository = DataAccessFactory.CreateRepository<TIREPOSITORY>(UnitOfWork);
                    TENTITY Entity = Repository.Get(e => e.Id == Id, IncludeProperties);

                    TDTO Dto = Mapper.Map<TENTITY, TDTO>(Entity);
                    return Dto;
                }
            }
        }

        public IList<TDTO> GetAll()
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    TIREPOSITORY Repository = DataAccessFactory.CreateRepository<TIREPOSITORY>(UnitOfWork);
                    IList<TENTITY> EntityList = Repository.GetAsIEnumerable(e => e.Id != null).ToList();

                    IList<TDTO> DtoList = Mapper.Map<IList<TENTITY>, IList<TDTO>>(EntityList);
                    return DtoList;
                }
            }
        }

        public Guid Save(TDTO Dto, string IncludeProperties = "")
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    // Hook to allow modification in derived implementations
                    BeforeSave(ref Dto, UnitOfWork);

                    TIREPOSITORY Repository = DataAccessFactory.CreateRepository<TIREPOSITORY>(UnitOfWork);

                    if (Dto.IsNew)
                    {
                        // Hook to allow modification in derived implementations
                        BeforeInsert(ref Dto, UnitOfWork);
                        Dto.IsNew = true;

                        TENTITY Entity = Mapper.Map<TDTO, TENTITY>(Dto);
                        Repository.Insert(Entity);
                    }
                    else
                    {
                        // Hook to allow modification in derived implementations
                        BeforeUpdate(ref Dto, UnitOfWork);
                        Dto.IsNew = false;

                        TENTITY Entity = Repository.Get(Dto.Id, IncludeProperties);

                        if (!Dto.RowVersion.SequenceEqual(Entity.RowVersion))
                            throw new ConcurrencyException(string.Format("The entity, {0}, was modified between when it was read and when this write was attempted. Please reprocess.", Entity));

                        Entity = Mapper.Map<TDTO, TENTITY>(Dto, Entity);
                        Repository.Update(Entity);
                    }

                    UnitOfWork.Commit();
                    return Dto.Id;
                }
            }
        }

        public virtual void Delete(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    TIREPOSITORY Repository = DataAccessFactory.CreateRepository<TIREPOSITORY>(UnitOfWork);
                    Repository.Delete(Id);

                    UnitOfWork.Commit();
                }
            }
        }

        virtual protected void BeforeSave(ref TDTO Dto, IUnitOfWork UnitOfWork) { ;  }
        virtual protected void BeforeInsert(ref TDTO Dto, IUnitOfWork UnitOfWork) { ; }
        virtual protected void BeforeUpdate(ref TDTO Dto, IUnitOfWork UnitOfWork) { ; }
    }
}
