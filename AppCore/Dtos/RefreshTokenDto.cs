using AppCore.BaseModel;

namespace AppCore.Dtos
{
    /// <summary>
    /// Data transfer object for refresh token information
    /// </summary>
    public class RefreshTokenDto : BaseDto
    {
        /// <summary>
        /// Unique token string
        /// </summary>
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// Date and time when the token expires
        /// </summary>
        public DateTime ExpiryDate { get; set; }
        
        /// <summary>
        /// Whether the token has been revoked
        /// </summary>
        public bool IsRevoked { get; set; }
        
        /// <summary>
        /// Reason for token revocation (if applicable)
        /// </summary>
        public string RevokedReason { get; set; } = string.Empty;
        
        /// <summary>
        /// Token that replaced this one (if applicable)
        /// </summary>
        public string ReplacedByToken { get; set; } = string.Empty;

        /// <summary>
        /// User ID associated with this token
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// IP address that created this token
        /// </summary>
        public string CreatedByIp { get; set; } = string.Empty;
        
        /// <summary>
        /// IP address that revoked this token (if applicable)
        /// </summary>
        public string RevokedByIp { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether the token is still active
        /// </summary>
        public bool IsActive { get; set; }
    }
}