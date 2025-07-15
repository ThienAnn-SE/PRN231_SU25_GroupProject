using System;
using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class RefreshToken : BaseEntity
    {
        public RefreshToken() { }

        public RefreshToken(string token, DateTime expiryDate, Guid userId, string createdByIp)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            ExpiryDate = expiryDate;
            UserId = userId;
            CreatedByIp = createdByIp ?? throw new ArgumentNullException(nameof(createdByIp));
            IsRevoked = false;
        }

        /// <summary>Unique token string</summary>
        public string Token { get; private set; }

        /// <summary>Date and time when the token expires</summary>
        public DateTime ExpiryDate { get; private set; }

        /// <summary>Whether the token has been revoked</summary>
        public bool IsRevoked { get; private set; }

        /// <summary>Reason for token revocation (if applicable)</summary>
        public string? RevokedReason { get; private set; }

        /// <summary>Token that replaced this one (if applicable)</summary>
        public string? ReplacedByToken { get; private set; }

        /// <summary>User ID associated with this token</summary>
        public Guid UserId { get; private set; }

        /// <summary>IP address that created this token</summary>
        public string CreatedByIp { get; private set; }

        /// <summary>IP address that revoked this token (if applicable)</summary>
        public string? RevokedByIp { get; private set; }

        /// <summary>Whether the token is still active</summary>
        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiryDate;

        /// <summary>
        /// Revokes this token
        /// </summary>
        /// <param name="reason">Reason for revocation</param>
        /// <param name="revokedByIp">IP address performing the revocation</param>
        /// <param name="replacedByToken">Token replacing this one (if applicable)</param>
        public void Revoke(string? reason = null, string? revokedByIp = null, string? replacedByToken = null)
        {
            IsRevoked = true;
            RevokedReason = reason;
            RevokedByIp = revokedByIp;
            ReplacedByToken = replacedByToken;
        }

        /// <summary>
        /// Creates a new token that replaces this one
        /// </summary>
        /// <param name="newToken">The new token value</param>
        /// <param name="ipAddress">IP address performing the rotation</param>
        /// <returns>A new refresh token</returns>
        public RefreshToken RotateToken(string newToken, string ipAddress)
        {
            if (string.IsNullOrEmpty(newToken))
                throw new ArgumentNullException(nameof(newToken));
            
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            var newRefreshToken = new RefreshToken(
                newToken,
                DateTime.UtcNow.AddDays(7),
                UserId,
                ipAddress
            );

            // Revoke the current token
            Revoke("Replaced by new token", ipAddress, newToken);

            return newRefreshToken;
        }
    }

    public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.Property(x => x.Token).IsRequired(true).HasMaxLength(512);
            builder.Property(x => x.ExpiryDate).IsRequired(true);
            builder.Property(x => x.IsRevoked).IsRequired(true).HasDefaultValue(false);
            builder.Property(x => x.RevokedReason).IsRequired(false);
            builder.Property(x => x.ReplacedByToken).IsRequired(false);
            builder.Property(x => x.UserId).IsRequired(true);
            builder.Property(x => x.CreatedByIp).IsRequired(true).HasMaxLength(45);
            builder.Property(x => x.RevokedByIp).IsRequired(false).HasMaxLength(45);

            // Index for faster querying by user
            builder.HasIndex(x => x.UserId);
            // Index for faster token lookups
            builder.HasIndex(x => x.Token).IsUnique();
        }
    }
}
