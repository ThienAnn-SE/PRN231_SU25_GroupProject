using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repositories
{
    /// <summary>
    /// Repository for working with ExternalLogin data using DTOs
    /// </summary>
    public interface IExternalLoginRepository
    {
        /// <summary>
        /// Gets an external login by ID
        /// </summary>
        Task<ExternalLoginDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets external login by provider and provider key
        /// </summary>
        Task<ExternalLoginDto?> GetByLoginInfoAsync(string provider, string providerKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all external logins for a user
        /// </summary>
        Task<List<ExternalLoginDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new external login
        /// </summary>
        Task<bool> CreateAsync(ExternalLoginDto externalLoginDto, Guid? creatorId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an external login
        /// </summary>
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Repository implementation for working with external logins
    /// </summary>
    public class ExternalLoginRepository : IExternalLoginRepository
    {
        private readonly CrudRepository<ExternalLogin> _repository;

        public ExternalLoginRepository(
            DbContext dbContext, 
            IDbTransaction transaction)
        {
            _repository = new CrudRepository<ExternalLogin>(dbContext, transaction);
        }

        /// <summary>
        /// Gets an external login by ID
        /// </summary>
        public async Task<ExternalLoginDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.FindByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return null;
            }
            return new ExternalLoginDto()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Provider = entity.Provider,
                ProviderKey = entity.ProviderKey,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        /// <summary>
        /// Gets external login by provider and provider key
        /// </summary>
        public async Task<ExternalLoginDto?> GetByLoginInfoAsync(string provider, string providerKey, CancellationToken cancellationToken = default)
        {
            var filters = new Expression<Func<ExternalLogin, bool>>[]
            {
                x => x.Provider == provider,
                x => x.ProviderKey == providerKey
            };
            
            var entity = await _repository.FindOneAsync(filters, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return null;
            }
            return new ExternalLoginDto()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Provider = entity.Provider,
                ProviderKey = entity.ProviderKey,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        /// <summary>
        /// Gets all external logins for a user
        /// </summary>
        public async Task<List<ExternalLoginDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var filters = new Expression<Func<ExternalLogin, bool>>[]
            {
                x => x.UserId == userId
            };
            
            var entities = await _repository.FindAsync(filters, "Provider", cancellationToken: cancellationToken);
            return [.. entities.Select(x => new ExternalLoginDto()
            {
                Id = x.Id,
                UserId = x.UserId,
                Provider = x.Provider,
                ProviderKey = x.ProviderKey,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })]; ;
        }

        /// <summary>
        /// Creates a new external login
        /// </summary>
        public async Task<bool> CreateAsync(ExternalLoginDto externalLoginDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            //var entity = _mapper.MapToEntity(externalLoginDto);
            return await _repository.SaveAsync(new ExternalLogin(externalLoginDto.ProviderKey, externalLoginDto.Provider, externalLoginDto.UserId), creatorId, cancellationToken);
        }

        /// <summary>
        /// Deletes an external login
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.HardDeleteAsync(id, cancellationToken);
        }
    }
}