using AppCore.BaseModel;

namespace AppCore.Dtos
{
    /// <summary>
    /// Data transfer object for external login information
    /// </summary>
    public class ExternalLoginDto : BaseDto
    {
        /// <summary>
        /// Name of the authentication provider (e.g., "Google", "Microsoft")
        /// </summary>
        public string Provider { get; set; } = string.Empty;
        
        /// <summary>
        /// The provider's unique identifier for the user
        /// </summary>
        public string ProviderKey { get; set; } = string.Empty;

        /// <summary>
        /// Associated user ID in the application
        /// </summary>
        public Guid UserId { get; set; }
    }
}