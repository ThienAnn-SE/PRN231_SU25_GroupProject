using AppCore.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
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
}
