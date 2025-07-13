using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repositories
{
    /// <summary>
    /// Repository for working with RefreshToken data using DTOs
    /// </summary>
    public interface IRefreshTokenRepository
    {
        /// <summary>
        /// Gets a refresh token by ID
        /// </summary>
        Task<RefreshTokenDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a refresh token by token value
        /// </summary>
        Task<RefreshTokenDto?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets active tokens for a user
        /// </summary>
        Task<List<RefreshTokenDto>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new refresh token
        /// </summary>
        Task<bool> CreateAsync(RefreshTokenDto refreshTokenDto, Guid? creatorId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a refresh token
        /// </summary>
        Task<bool> RevokeAsync(Guid id, string? reason, string? revokedByIp, Guid? editorId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces an existing refresh token with a new one
        /// </summary>
        Task<RefreshTokenDto?> RotateTokenAsync(Guid id, string newToken, string ipAddress, Guid? editorId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a refresh token
        /// </summary>
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Repository implementation for working with refresh tokens
    /// </summary>
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly CrudRepository<RefreshToken> _repository;

        public RefreshTokenRepository(
            DbContext dbContext, 
            IDbTransaction transaction)
        {
            _repository = new CrudRepository<RefreshToken>(dbContext, transaction);
        }

        /// <summary>
        /// Gets a refresh token by ID
        /// </summary>
        public async Task<RefreshTokenDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.FindByIdAsync(id, cancellationToken);
            return new RefreshTokenDto()
            {
                Id = entity?.Id ?? Guid.Empty,
                Token = entity?.Token ?? string.Empty,
                ExpiryDate = entity?.ExpiryDate ?? DateTime.MinValue,
                IsRevoked = entity?.IsRevoked ?? false,
                RevokedReason = entity?.RevokedReason ?? string.Empty,
                ReplacedByToken = entity?.ReplacedByToken ?? string.Empty,
                UserId = entity?.UserId ?? Guid.Empty,
                CreatedByIp = entity?.CreatedByIp ?? string.Empty,
                RevokedByIp = entity?.RevokedByIp ?? string.Empty,
                IsActive = entity?.IsActive ?? false
            } ;
        }

        /// <summary>
        /// Gets a refresh token by token value
        /// </summary>
        public async Task<RefreshTokenDto?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<RefreshToken, bool>>[]
            {
                x => x.Token == token
            };
            
            var entity = await _repository.FindOneAsync(filter, cancellationToken: cancellationToken);
            return new RefreshTokenDto()
            {
                Id = entity?.Id ?? Guid.Empty,
                Token = entity?.Token ?? string.Empty,
                ExpiryDate = entity?.ExpiryDate ?? DateTime.MinValue,
                IsRevoked = entity?.IsRevoked ?? false,
                RevokedReason = entity?.RevokedReason ?? string.Empty,
                ReplacedByToken = entity?.ReplacedByToken ?? string.Empty,
                UserId = entity?.UserId ?? Guid.Empty,
                CreatedByIp = entity?.CreatedByIp ?? string.Empty,
                RevokedByIp = entity?.RevokedByIp ?? string.Empty,
                IsActive = entity?.IsActive ?? false
            };
        }

        /// <summary>
        /// Gets active tokens for a user
        /// </summary>
        public async Task<List<RefreshTokenDto>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var filters = new Expression<Func<RefreshToken, bool>>[]
            {
                x => x.UserId == userId,
                x => !x.IsRevoked,
                x => x.ExpiryDate > DateTime.UtcNow
            };
            
            var entities = await _repository.FindAsync(filters, "ExpiryDate desc", cancellationToken: cancellationToken);
            return [.. entities.Select(entity => new RefreshTokenDto()
            {
                Id = entity?.Id ?? Guid.Empty,
                Token = entity?.Token ?? string.Empty,
                ExpiryDate = entity?.ExpiryDate ?? DateTime.MinValue,
                IsRevoked = entity?.IsRevoked ?? false,
                RevokedReason = entity?.RevokedReason ?? string.Empty,
                ReplacedByToken = entity?.ReplacedByToken ?? string.Empty,
                UserId = entity?.UserId ?? Guid.Empty,
                CreatedByIp = entity?.CreatedByIp ?? string.Empty,
                RevokedByIp = entity?.RevokedByIp ?? string.Empty,
                IsActive = entity?.IsActive ?? false
            })];
        }

        /// <summary>
        /// Creates a new refresh token
        /// </summary>
        public async Task<bool> CreateAsync(RefreshTokenDto refreshTokenDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            // var entity = _mapper.MapToEntity(refreshTokenDto);
            var entity = new RefreshToken(refreshTokenDto.Token, refreshTokenDto.ExpiryDate, refreshTokenDto.UserId, refreshTokenDto.CreatedByIp)
            {
                Id = Guid.NewGuid()
            };
            return await _repository.SaveAsync(entity, creatorId, cancellationToken);
        }

        /// <summary>
        /// Revokes a refresh token
        /// </summary>
        public async Task<bool> RevokeAsync(Guid id, string? reason, string? revokedByIp, Guid? editorId = null, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.FindByIdAsync(id, cancellationToken);
            if (entity == null)
                return false;
            
            entity.Revoke(reason, revokedByIp);
            return await _repository.UpdateAsync(entity, editorId, cancellationToken);
        }

        /// <summary>
        /// Replaces an existing refresh token with a new one
        /// </summary>
        public async Task<RefreshTokenDto?> RotateTokenAsync(Guid id, string newToken, string ipAddress, Guid? editorId = null, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.FindByIdAsync(id, cancellationToken);
            if (entity == null)
                return null;
            
            var newRefreshToken = entity.RotateToken(newToken, ipAddress);
            await _repository.UpdateAsync(entity, editorId, cancellationToken);
            await _repository.SaveAsync(newRefreshToken, editorId, cancellationToken);
            
            return new RefreshTokenDto()
            {
                Id = newRefreshToken?.Id ?? Guid.Empty,
                Token = newRefreshToken?.Token ?? string.Empty,
                ExpiryDate = newRefreshToken?.ExpiryDate ?? DateTime.MinValue,
                IsRevoked = newRefreshToken?.IsRevoked ?? false,
                RevokedReason = newRefreshToken?.RevokedReason ?? string.Empty,
                ReplacedByToken = newRefreshToken?.ReplacedByToken ?? string.Empty,
                UserId = newRefreshToken?.UserId ?? Guid.Empty,
                CreatedByIp = newRefreshToken?.CreatedByIp ?? string.Empty,
                RevokedByIp = newRefreshToken?.RevokedByIp ?? string.Empty,
                IsActive = newRefreshToken?.IsActive ?? false
            };
        }

        /// <summary>
        /// Deletes a refresh token
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.HardDeleteAsync(id, cancellationToken);
        }
    }
}