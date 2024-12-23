using Core.Domain.Base;
using Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Core.Infrastructure.Persistences
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DbContext _context;
        private readonly ILogger<GenericRepository<T>> _logger;

        public GenericRepository(DbContext context, ILogger<GenericRepository<T>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Add(T item)
        {
            _logger.LogInformation("ADDING {Object} entity to database", typeof(T).Name);

            await _context.Set<T>().AddAsync(item);
        }

        public void Delete(T item)
        {
            _logger.LogInformation("DELETING {Object} entity from database", typeof(T).Name);

            _context.Set<T>().Remove(item);
        }

        public void UpdateRange(List<T> items)
        {
            _logger.LogInformation("UPDATING {Object} entities from database", typeof(T).Name);

            _context.Set<T>().UpdateRange(items);
        }

        public async Task<List<T>> FindByQuery(Expression<Func<T, bool>>? expression, bool tracking = true, params Expression<Func<T, object>>[]? includeProperties)
        {
            _logger.LogInformation("RETRIEVING {Object} entities from database by {query}", typeof(T).Name, expression?.ToString() ?? string.Empty);

            var queryable = (IQueryable<T>)_context.Set<T>();

            if (tracking is false)
                queryable = queryable.AsNoTracking();

            if (includeProperties.Any())
            {
                queryable = includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
            }

            return expression is null ? await queryable.ToListAsync() : await queryable.Where(expression).ToListAsync();
        }

        public async Task<T> GetById(long id, params Expression<Func<T, object>>[]? includeProperties)
        {
            _logger.LogInformation("RETRIEVING {Object} entitie from database by id:{Id}", typeof(T).Name, id);

            var queryable = (IQueryable<T>)_context.Set<T>();

            if (includeProperties.Any())
            {
                queryable = includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
            }

            return await queryable.SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<T> FindEntityByQuery(Expression<Func<T, bool>> expression, bool tracking = true, params Expression<Func<T, object>>[]? includeProperties)
        {
            _logger.LogInformation("RETRIEVING {Object} entities from database by {query}", typeof(T).Name, expression?.ToString() ?? string.Empty);

            var queryable = (IQueryable<T>)_context.Set<T>();

            if (tracking is false)
                queryable = queryable.AsNoTracking();

            if (includeProperties.Any())
            {
                queryable = includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
            }

            return await queryable.SingleOrDefaultAsync(expression);
        }

        public async Task AddRange(List<T> items)
        {
            _logger.LogInformation("ADDING {Object} entities to database", typeof(T).Name);

            await _context.Set<T>().AddRangeAsync(items);
        }

        public void Update(T item)
        {

            _logger.LogInformation("UPDATING {Object} entity from database", typeof(T).Name);

            _context.Set<T>().UpdateRange(item);
        }

        public async Task<List<T>> FindByQueryPaging(Expression<Func<T, bool>> expression, bool tracking = true, int pageIndex = 0, int pageSize = 0, params Expression<Func<T, object>>[]? includeProperties)
        {
            _logger.LogInformation("RETRIEVING {Object} entities from database with paging by {query}, pageIndex:{PageIndex}, pageSize{PageSize}",
                typeof(T).Name, expression?.ToString() ?? string.Empty, pageIndex, pageSize);

            var queryable = (IQueryable<T>)_context.Set<T>();

            if (tracking is false)
                queryable = queryable.AsNoTracking();

            if (includeProperties.Any())
            {
                queryable = includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
            }


            if (expression is not null)
            {
                queryable = queryable.Where(expression);
            }

            queryable = queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return queryable.ToList();
        }

        public void DeleteRange(List<T> items)
        {
            _logger.LogInformation("DELETING {Object} entities from database", typeof(T).Name);

            _context.Set<T>().RemoveRange(items);
        }
    }
}
