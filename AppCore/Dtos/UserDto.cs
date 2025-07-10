using AppCore.BaseModel;

namespace AppCore.Dtos
{
    /// <summary>
    /// Data transfer object for user authentication information
    /// </summary>
    public class UserDto : BaseDto
    {
        /// <summary>
        /// User's login name
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Whether the email has been confirmed
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Whether two-factor authentication is enabled
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Time until which the account is locked
        /// </summary>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Number of consecutive failed access attempts
        /// </summary>
        public int AccessFailedCount { get; set; }
    }

    public class LoginDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

}