using AppCore.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
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
}
