using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AppCore
{
    /// <summary>
    /// Interface for repository operations that modify data
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface ICrudRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Checks if an entity with the specified ID exists
        /// </summary>
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Finds an entity by its ID
        /// </summary>
        Task<TEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets all non-deleted entities
        /// </summary>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Saves a new entity
        /// </summary>
        Task<bool> SaveAsync(TEntity entity, Guid? creatorId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Saves multiple entities
        /// </summary>
        Task<bool> SaveAllAsync(IEnumerable<TEntity> entities, Guid? creatorId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Updates an existing entity
        /// </summary>
        Task<bool> UpdateAsync(TEntity entity, Guid? editorId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Soft deletes an entity by ID
        /// </summary>
        Task<bool> SoftDeleteAsync(Guid id, Guid? editorId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Soft deletes an entity
        /// </summary>
        Task<bool> SoftDeleteAsync(TEntity entity, Guid? editorId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Permanently deletes an entity by ID
        /// </summary>
        Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Permanently deletes an entity
        /// </summary>
        Task<bool> HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Implementation for CRUD repository operations
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class CrudRepository<TEntity> : ReadOnlyRepository<TEntity>, ICrudRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IDbTransaction _dbTransaction;
        private readonly DbContext _dbContext;

        public CrudRepository(DbContext dbContext, IDbTransaction dbTransaction) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbTransaction = dbTransaction ?? throw new ArgumentNullException(nameof(dbTransaction));
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(x => x.Id == id && !x.DeletedAt.HasValue, cancellationToken);
        }

        public async Task<TEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
           var query = IncludeProperties(_dbSet);
            return await query.FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var query = IncludeProperties(_dbSet).Where(x => !x.DeletedAt.HasValue);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<bool> SaveAsync(TEntity entity, Guid? creatorId, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var now = DateTime.UtcNow;
            entity.CreatedAt = now;
            entity.UpdatedAt = now;
            entity.CreatorId = creatorId;
            
            return await _dbTransaction.ExecuteInTransactionAsync(async () =>
            {
                await _dbSet.AddAsync(entity, cancellationToken);
            }, cancellationToken);
        }

        public async Task<bool> SaveAllAsync(IEnumerable<TEntity> entities, Guid? creatorId, CancellationToken cancellationToken = default)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            
            var entitiesList = entities.ToList();
            if (!entitiesList.Any()) throw new ArgumentException("Entities collection cannot be empty.", nameof(entities));
            
            var now = DateTime.UtcNow;
            foreach (var entity in entitiesList)
            {
                entity.CreatedAt = now;
                entity.UpdatedAt = now;
                entity.CreatorId = creatorId;
            }
            
            return await _dbTransaction.ExecuteInTransactionAsync(async () =>
            {
                await _dbSet.AddRangeAsync(entitiesList, cancellationToken);
            }, cancellationToken);
        }

        public async Task<bool> UpdateAsync(TEntity entity, Guid? editorId, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return await _dbTransaction.ExecuteInTransactionAsync(() =>
            {
                entity.UpdatedAt = DateTime.UtcNow;
                entity.EditorId = editorId;
                
                _dbContext.Entry(entity).State = EntityState.Modified;
                
                // Ensure these fields are not modified
                _dbContext.Entry(entity).Property(e => e.CreatedAt).IsModified = false;
                _dbContext.Entry(entity).Property(e => e.CreatorId).IsModified = false;
            }, cancellationToken);
        }

        public async Task<bool> SoftDeleteAsync(Guid id, Guid? editorId, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken)
                ?? throw new KeyNotFoundException($"Entity with ID {id} not found.");
                
            return await SoftDeleteAsync(entity, editorId, cancellationToken);
        }

        public async Task<bool> SoftDeleteAsync(TEntity entity, Guid? editorId, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            return await _dbTransaction.ExecuteInTransactionAsync(() =>
            {
                var now = DateTime.UtcNow;
                entity.DeletedAt = now;
                entity.UpdatedAt = now;
                entity.EditorId = editorId;
                
                _dbContext.Entry(entity).State = EntityState.Modified;
            }, cancellationToken);
        }

        public async Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken)
                ?? throw new KeyNotFoundException($"Entity with ID {id} not found.");
                
            return await HardDeleteAsync(entity, cancellationToken);
        }

        public async Task<bool> HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            return await _dbTransaction.ExecuteInTransactionAsync(() =>
            {
                _dbSet.Remove(entity);
            }, cancellationToken);
        }
    }
}
