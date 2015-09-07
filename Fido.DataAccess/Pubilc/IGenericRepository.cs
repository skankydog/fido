// From the perspective of a Repository Pattern, you can think of it this way:
//
// 1. Use an eager loading IEnumerable when you want To pass an entire list To the client in one
//    go. They can still add linq clauses, but the client does not benefit From deferred execution.
//
// 2. Use a lazy loading IQueryable when you want To extend deferred querying capabilities To the
//    client, by allowing the client To add their own linq clauses. This defers execution of the
//    entire query until output is required.
// (http://stackoverflow.com/questions/8947923/iqueryable-vs-ienumerable-in-the-repository-pattern-lazy-loading)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fido.Entities;

namespace Fido.DataAccess
{
    public interface IGenericRepository<TENTITY>
        where TENTITY : Entity
    {
        TENTITY Get(Guid Id, string IncludeProperties = "");
        TENTITY Get(
            Expression<Func<TENTITY, bool>> Predicate,
            string IncludeProperties = "");
        IEnumerable<TENTITY> GetAsIEnumerable(
            Expression<Func<TENTITY, bool>> Predicate = null,
            Func<IQueryable<TENTITY>, IOrderedQueryable<TENTITY>> OrderBy = null,
            string IncludeProperties = "");
        IQueryable<TENTITY> GetAsIQueryable(
            Expression<Func<TENTITY, bool>> Predicate = null,
            Func<IQueryable<TENTITY>, IOrderedQueryable<TENTITY>> OrderBy = null,
            string IncludeProperties = "");
        TENTITY Insert(TENTITY Entity);
        TENTITY Update(TENTITY Entity);
        void Delete(Guid Id);
        void Delete(TENTITY Entity);
        void Delete(Expression<Func<TENTITY, bool>> Predicate);

        void SetUnique(string FieldName);
        void Index(string FieldName);
    }
}
