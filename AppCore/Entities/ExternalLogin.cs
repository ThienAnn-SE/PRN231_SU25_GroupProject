using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AppCore.Entities
{
    /// <summary>
    /// Represents an external authentication provider login linked to a user account
    /// </summary>
    public class ExternalLogin : BaseEntity
    {
        /// <summary>
        /// Protected constructor for EF Core
        /// </summary>
        protected ExternalLogin() { }

        /// <summary>
        /// Creates a new external login
        /// </summary>
        /// <param name="provider">Authentication provider name (e.g., "Google", "Microsoft")</param>
        /// <param name="providerKey">External provider's unique identifier for the user</param>
        /// <param name="userId">Associated user ID</param>
        public ExternalLogin(string provider, string providerKey, Guid userId)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            ProviderKey = providerKey ?? throw new ArgumentNullException(nameof(providerKey));
            UserId = userId;
        }

        /// <summary>Name of the authentication provider (e.g., "Google", "Microsoft")</summary>
        public string Provider { get; private set; }
        
        /// <summary>The provider's unique identifier for the user</summary>
        public string ProviderKey { get; private set; }
        
        /// <summary>Associated user ID in the application</summary>
        public Guid UserId { get; private set; }
    }
    
    /// <summary>
    /// Entity Framework configuration for ExternalLogin
    /// </summary>
    public class ExternalLoginConfig : IEntityTypeConfiguration<ExternalLogin>
    {
        public void Configure(EntityTypeBuilder<ExternalLogin> builder)
        {
            builder.Property(x => x.Provider).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ProviderKey).IsRequired().HasMaxLength(255);
            builder.Property(x => x.UserId).IsRequired();
            
            // Create a composite unique index on Provider and ProviderKey
            builder.HasIndex(x => new { x.Provider, x.ProviderKey }).IsUnique();
            
            // Create an index on UserId for faster lookups
            builder.HasIndex(x => x.UserId);
        }
    }
}