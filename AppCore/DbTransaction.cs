using Microsoft.EntityFrameworkCore;

namespace AppCore
{
    /// <summary>
    /// Provides transaction management for database operations
    /// </summary>
    public interface IDbTransaction : IDisposable
    {
        /// <summary>
        /// Executes an asynchronous operation within a transaction
        /// </summary>
        /// <param name="operation">The asynchronous operation to execute</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>True if operation succeeded and changes were saved, false otherwise</returns>
        Task<bool> ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a synchronous operation within a transaction
        /// </summary>
        /// <param name="operation">The synchronous operation to execute</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>True if operation succeeded and changes were saved, false otherwise</returns>
        Task<bool> ExecuteInTransactionAsync(Action operation, CancellationToken cancellationToken = default);
    }

    public class DbTransaction : IDbTransaction
    {
        private readonly DbContext _context;

        public DbTransaction(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await operation();
                var rowsAffected = await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return rowsAffected > 0; // Only consider success when rows are affected
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<bool> ExecuteInTransactionAsync(Action operation, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                operation();
                var rowsAffected = await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return rowsAffected > 0; // Only consider success when rows are affected
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public void Dispose()
        {
            // Context is injected, so we don't dispose it here
            // This class doesn't own any disposable resources itself
            GC.SuppressFinalize(this);
        }
    }
}
