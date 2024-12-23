using Core.Domain.Base;
using System.Linq.Expressions;

namespace Core.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task Add(T item);
        void Delete(T item);
        void DeleteRange(List<T> items);
        Task<T> GetById(long id, params Expression<Func<T, object>>[]? includeProperties);
        Task<List<T>> FindByQuery(Expression<Func<T, bool>> expression, bool tracking = true, params Expression<Func<T, object>>[]? includeProperties);


        Task<List<T>> FindByQueryPaging(Expression<Func<T, bool>> expression, bool tracking = true, int pageIndex = default, int pageSize = default, params Expression<Func<T, object>>[]? includeProperties);

        Task<T> FindEntityByQuery(Expression<Func<T, bool>> expression, bool tracking = true, params Expression<Func<T, object>>[]? includeProperties);
        void Update(T item);
        void UpdateRange(List<T> items);
        Task AddRange(List<T> items);
    }
}
