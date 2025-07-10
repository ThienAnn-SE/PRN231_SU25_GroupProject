using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCore
{
    /// <summary>
    /// Represents a repository that provides read-only operations on entities
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public interface IReadOnlyRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Finds a single entity that matches the specified filters
        /// </summary>
        /// <param name="filters">Array of filter expressions</param>
        /// <param name="orderBy">Property name with optional direction (e.g. "Name", "CreatedAt desc")</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The matched entity or null if not found</returns>
        Task<TEntity?> FindOneAsync(
            Expression<Func<TEntity, bool>>[]? filters,
            string? orderBy = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds entities that match the specified filters with pagination
        /// </summary>
        /// <param name="filters">Array of filter expressions</param>
        /// <param name="orderBy">Property name with optional direction (e.g. "Name", "CreatedAt desc")</param>
        /// <param name="skip">Number of entities to skip</param>
        /// <param name="limit">Maximum number of entities to return</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of matched entities</returns>
        Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>>[]? filters,
            string? orderBy = null,
            int skip = 0,
            int limit = 0,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts entities that match the specified filters
        /// </summary>
        /// <param name="filters">Array of filter expressions</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Count of matched entities</returns>
        Task<int> CountAsync(
            Expression<Func<TEntity, bool>>[]? filters = null,
            CancellationToken cancellationToken = default);
            
        /// <summary>
        /// Gets a paged result containing entities and total count
        /// </summary>
        /// <param name="filters">Array of filter expressions</param>
        /// <param name="orderBy">Property name with optional direction (e.g. "Name", "CreatedAt desc")</param>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result with entities and metadata</returns>
        Task<PagedResult<TEntity>> GetPagedAsync(
            Expression<Func<TEntity, bool>>[]? filters = null,
            string? orderBy = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);
    }
    
    /// <summary>
    /// Contains paged result data and metadata
    /// </summary>
    /// <typeparam name="T">Type of items in the result</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Items in the current page
        /// </summary>
        public List<T> Items { get; }
        
        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; }
        
        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int Page { get; }
        
        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; }
        
        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        
        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPrevious => Page > 1;
        
        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNext => Page < TotalPages;
        
        public PagedResult(List<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }

    /// <summary>
    /// Provides read-only operations on entities
    /// </summary>
    public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DbSet<TEntity> _dbSet;

        public ReadOnlyRepository(DbContext dbContext)
        {
            _dbSet = dbContext?.Set<TEntity>() ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<TEntity?> FindOneAsync(
            Expression<Func<TEntity, bool>>[]? filters,
            string? orderBy = null,
            CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(filters, orderBy);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>>[]? filters,
            string? orderBy = null,
            int skip = 0,
            int limit = 0,
            CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(filters, orderBy);

            if (skip > 0)
                query = query.Skip(skip);

            if (limit > 0)
                query = query.Take(limit);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(
            Expression<Func<TEntity, bool>>[]? filters = null,
            CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(filters, null);
            return await query.CountAsync(cancellationToken);
        }
        
        public async Task<PagedResult<TEntity>> GetPagedAsync(
            Expression<Func<TEntity, bool>>[]? filters = null,
            string? orderBy = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            
            var query = BuildQuery(filters, orderBy);
            
            var totalCount = await query.CountAsync(cancellationToken);
            var skip = (page - 1) * pageSize;
            
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
                
            return new PagedResult<TEntity>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Builds a query with the specified filters and ordering
        /// </summary>
        protected IQueryable<TEntity> BuildQuery(Expression<Func<TEntity, bool>>[]? filters, string? orderBy)
        {
            // Start with a query that excludes deleted items
            var query = _dbSet.Where(x => !x.DeletedAt.HasValue);

            // Apply additional filters if provided
            if (filters != null && filters.Length > 0)
            {
                query = filters.Aggregate(query, (current, filter) => current.Where(filter));
            }

            // Apply ordering if provided
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query = ApplyOrderBy(query, orderBy);
            }

            return query;
        }

        /// <summary>
        /// Applies ordering to a query based on a property name and optional direction
        /// </summary>
        protected IQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query, string orderByExpression)
        {
            try
            {
                var parts = orderByExpression.Split(' ');
                var propertyName = parts[0].Trim();
                var descending = parts.Length > 1 && parts[1].Trim().Equals("desc", StringComparison.OrdinalIgnoreCase);

                // Validate property exists
                var property = typeof(TEntity).GetProperty(propertyName) 
                    ?? throw new ArgumentException($"Property '{propertyName}' not found on type {typeof(TEntity).Name}");

                // Apply ordering
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var propertyAccess = Expression.Property(parameter, property);
                var lambda = Expression.Lambda(propertyAccess, parameter);

                string methodName = descending ? "OrderByDescending" : "OrderBy";
                var orderByMethod = typeof(Queryable).GetMethods()
                    .Where(m => m.Name == methodName && m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
                    .Single();

                var genericMethod = orderByMethod.MakeGenericMethod(typeof(TEntity), property.PropertyType);
                return (IQueryable<TEntity>)genericMethod.Invoke(null, new object[] { query, lambda })!;
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                // Fall back to ordering by Id if there's an error
                return query.OrderBy(e => e.Id);
            }
        }
    }
}