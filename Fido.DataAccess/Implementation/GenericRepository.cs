using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.DataAccess;
using Fido.Entities;
using Fido.Core;

namespace Fido.DataAccess.Implementation
{
    internal abstract class GenericRepository<TENTITY> : IGenericRepository<TENTITY>
        where TENTITY : Entity
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        internal Context Context { get; private set; }

        public GenericRepository(IUnitOfWork UnitOfWork)
        {
            using (new FunctionLogger(Log))
            {
                if (UnitOfWork == null)
                {
                    Log.Fatal("The UnitOfWork parameter was set To null");

                    throw new ArgumentNullException("The UnitOfWork parameter passed To the GenericRepository is null. This is a non-optional parameter");
                }

                this.Context = ((UnitOfWork)UnitOfWork).Context;
            }
        }

        public virtual TENTITY Get(Guid Id, string IncludeProperties = "")
        {
            using (new FunctionLogger(Log))
            {
                return Get(e => e.Id == Id, IncludeProperties);
            }
        }

        public virtual TENTITY Get(Expression<Func<TENTITY, bool>> Predicate, string IncludeProperties = "")
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Predicate='{0}'", Predicate);
                return GetAsIQueryable(Predicate, null, IncludeProperties).FirstOrDefault();
            }
        }

        public virtual IEnumerable<TENTITY> GetAsIEnumerable(
            Expression<Func<TENTITY, bool>> Predicate= null,
            Func<IQueryable<TENTITY>, IOrderedQueryable<TENTITY>> OrderBy = null,
            string IncludeProperties = "")
        {
            using (new FunctionLogger(Log))
            {
                return GetAsIQueryable(Predicate, OrderBy, IncludeProperties).ToList();
            }
        }

        public virtual IQueryable<TENTITY> GetAsIQueryable(
            Expression<Func<TENTITY, bool>> Predicate = null,
            Func<IQueryable<TENTITY>, IOrderedQueryable<TENTITY>> OrderBy = null, // example: q => q.OrderBy(s => s.Id)
            string IncludeProperties = "")
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Predicate='{0}'", Predicate);
                Log.InfoFormat("OrderBy='{0}'", OrderBy);
                Log.InfoFormat("IncludeProperties='{0}'", IncludeProperties);

                IQueryable<TENTITY> l_Query = Context.Set<TENTITY>();

                if (Predicate != null)
                {
                    l_Query = l_Query.Where(Predicate);
                }

                if (IncludeProperties != null)
                {
                    foreach (var IncludeProperty in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        l_Query = l_Query.Include(IncludeProperty);
                }

                if (OrderBy != null)
                {
                    return OrderBy(l_Query);
                }
                else
                {
                    return l_Query;
                }
            }
        }

        public virtual void Insert(TENTITY Entity)
        {
            using (new FunctionLogger(Log))
            {
                // Added the below to ensure Id and Created Date/Time
                if (Entity.Id == null || Entity.Id == Guid.Empty)
                    Entity.Id = Guid.NewGuid();

                if (Entity.CreatedUtc == null || Entity.CreatedUtc == DateTime.MinValue)
                    Entity.CreatedUtc = DateTime.UtcNow;

                Log.InfoFormat("Entity.Id='{0}'", Entity.Id);
                Context.Set<TENTITY>().Add(Entity);
            }
        }

        public virtual void Update(TENTITY Entity)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Entity.Id='{0}'", Entity.Id);
                Context.Entry(Entity).State = EntityState.Modified;
            }
        }

        public virtual void Delete(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Id='{0}'", Id);
                TENTITY l_Entity = Context.Set<TENTITY>().Find(Id);
                Context.Set<TENTITY>().Remove(l_Entity);
            }
        }

        public virtual void Delete(TENTITY Entity)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Entity.Id='{0}'", Entity.Id);
                Context.Set<TENTITY>().Remove(Entity);
            }
        }

        public virtual void Delete(Expression<Func<TENTITY, bool>> Predicate)
        {
            using (new FunctionLogger(Log))
            {
                Log.InfoFormat("Predicate='{0}'", Predicate);

                var EntitiesToDelete =
                    from e in Context.Set<TENTITY>()
                    .Where(Predicate)
                    select e;

                foreach (TENTITY l_Entity in EntitiesToDelete)
                {
                    Context.Set<TENTITY>().Remove(l_Entity);
                }
            }
        }

        public virtual void SetUnique(string FieldName)
        {
            using (new FunctionLogger(Log))
            {
                string TableName = Context.GetTableName<TENTITY>();
                string EntityName = typeof(TENTITY).ToString().Split('.').Last();
                string SQL = string.Format(
                    @"IF NOT EXISTS(
                        SELECT * 
                        FROM sys.indexes 
                        WHERE name='IX_{0}_{1}' AND object_id = OBJECT_ID('{2}')) 
                    BEGIN 
                        CREATE UNIQUE INDEX IX_{0}_{1} ON {2} ({1}); 
                    END",
                    EntityName, FieldName, TableName);

                Log.DebugFormat("Table: {0}", TableName);
                Log.DebugFormat("Entity: {0}", EntityName);
                Log.DebugFormat("FieldName: {0}", FieldName);
                Log.DebugFormat("SQL: {0}", SQL);

                Context.Database.ExecuteSqlCommand(SQL);
            }
        }

        public virtual void Index(string FieldName)
        {
            using (new FunctionLogger(Log))
            {
                string TableName = Context.GetTableName<TENTITY>();
                string EntityName = typeof(TENTITY).ToString().Split('.').Last();
                string SQL = string.Format(
                    @"IF NOT EXISTS(
                        SELECT * 
                        FROM sys.indexes 
                        WHERE name='IX_{0}_{1} ' AND object_id = OBJECT_ID('{2}')) 
                    BEGIN 
                        CREATE INDEX IX_{0}_{1} ON {2} ({1}); 
                    END",
                    EntityName, FieldName, TableName);

                Log.DebugFormat("Table: {0}", TableName);
                Log.DebugFormat("Entity: {0}", EntityName);
                Log.DebugFormat("FieldName: {0}", FieldName);
                Log.DebugFormat("SQL: {0}", SQL);

                Context.Database.ExecuteSqlCommand(SQL);
            }
        }
    }
}
