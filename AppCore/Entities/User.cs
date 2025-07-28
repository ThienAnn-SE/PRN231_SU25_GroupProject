using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public enum UserRole
    {
        Admin,
        User,
        Manager
    }

    public class User : BaseEntity
    {
        protected User()
        {
            RefreshTokens = new List<RefreshToken>();
            ExternalLogins = new List<ExternalLogin>();
        }

        public User(string username, string email, string passwordHash, string salt)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Salt = salt ?? throw new ArgumentNullException(nameof(salt));
            EmailConfirmed = false;
            TwoFactorEnabled = false;
            AccessFailedCount = 0;
            RefreshTokens = new List<RefreshToken>();
            ExternalLogins = new List<ExternalLogin>();
        }

        /// <summary>User's login name</summary>
        public string Username { get; private set; }

        /// <summary>User's email address</summary>
        public string Email { get; private set; }

        /// <summary>Hashed password</summary>
        public string PasswordHash { get; private set; }

        /// <summary>Salt used for password hashing</summary>
        public string Salt { get; private set; }

        /// <summary>Whether the email has been confirmed</summary>
        public bool EmailConfirmed { get; private set; }

        /// <summary>Whether two-factor authentication is enabled</summary>
        public bool TwoFactorEnabled { get; private set; }

        /// <summary>Time until which the account is locked</summary>
        public DateTime? LockoutEnd { get; private set; }

        /// <summary>Number of consecutive failed access attempts</summary>
        public int AccessFailedCount { get; private set; }

        public UserRole Role { get; set; } = UserRole.User;

        /// <summary>List of refresh tokens associated with this user</summary>
        public List<RefreshToken> RefreshTokens { get; private set; }
        
        /// <summary>External login providers associated with this user</summary>
        public List<ExternalLogin> ExternalLogins { get; private set; }
        public void ConfirmEmail() => EmailConfirmed = true;

        public void EnableTwoFactor() => TwoFactorEnabled = true;

        public void DisableTwoFactor() => TwoFactorEnabled = false;

        public void UpdatePassword(string newPasswordHash, string newSalt)
        {
            PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));
            Salt = newSalt ?? throw new ArgumentNullException(nameof(newSalt));
        }

        public void RecordFailedAccess(int maxAttempts, TimeSpan lockoutTime)
        {
            AccessFailedCount++;
            if (AccessFailedCount >= maxAttempts)
            {
                LockoutEnd = DateTime.UtcNow.Add(lockoutTime);
            }
        }

        public void ResetAccessFailedCount() => AccessFailedCount = 0;

        /// <summary>
        /// Adds a refresh token to the user's collection
        /// </summary>
        /// <param name="token">Token to add</param>
        public void AddRefreshToken(RefreshToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            RefreshTokens.Add(token);
        }

        /// <summary>
        /// Removes expired refresh tokens from the user's collection
        /// </summary>
        /// <param name="maxTokens">Maximum number of tokens to keep (legacy parameter)</param>
        public void RemoveOldRefreshTokens(int maxTokens = 5)
        {
            // Find and remove only expired tokens
            var currentTime = DateTime.UtcNow;
            var expiredTokens = RefreshTokens
                .Where(t => currentTime >= t.ExpiryDate)
                .ToList();

            foreach (var token in expiredTokens)
            {
                RefreshTokens.Remove(token);
            }
        }
        
        /// <summary>
        /// Adds an external login to the user's account
        /// </summary>
        /// <param name="provider">Authentication provider</param>
        /// <param name="providerKey">Provider's unique identifier</param>
        /// <returns>The created external login</returns>
        public ExternalLogin AddExternalLogin(string provider, string providerKey)
        {
            if (string.IsNullOrEmpty(provider)) throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrEmpty(providerKey)) throw new ArgumentNullException(nameof(providerKey));
            
            // Check if this provider/key combination already exists
            if (ExternalLogins.Any(l => l.Provider == provider && l.ProviderKey == providerKey))
            {
                throw new InvalidOperationException($"External login for provider '{provider}' already exists");
            }
            
            var login = new ExternalLogin(provider, providerKey, Id);
            ExternalLogins.Add(login);
            return login;
        }
        
        /// <summary>
        /// Removes an external login from the user's account
        /// </summary>
        /// <param name="provider">Authentication provider</param>
        /// <param name="providerKey">Provider's unique identifier</param>
        /// <returns>True if login was found and removed, false otherwise</returns>
        public bool RemoveExternalLogin(string provider, string providerKey)
        {
            var login = ExternalLogins.FirstOrDefault(l => 
                l.Provider == provider && l.ProviderKey == providerKey);
                
            if (login != null)
            {
                return ExternalLogins.Remove(login);
            }
            
            return false;
        }
    }

    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Username).IsRequired(true).HasMaxLength(50);
            builder.Property(x => x.Email).IsRequired(true).HasMaxLength(255);
            builder.Property(x => x.PasswordHash).IsRequired(true);
            builder.Property(x => x.Salt).IsRequired(true);
            builder.Property(x => x.EmailConfirmed).IsRequired(true).HasDefaultValue(false);
            builder.Property(x => x.TwoFactorEnabled).IsRequired(true).HasDefaultValue(false);
            builder.Property(x => x.LockoutEnd).IsRequired(false).HasDefaultValue(null);
            builder.Property(x => x.AccessFailedCount).IsRequired(true).HasDefaultValue(0);
            builder.Property(x => x.Role).IsRequired(true).HasConversion<string>();

            // Configure relationship with RefreshTokens
            builder.HasMany(x => x.RefreshTokens)
                   .WithOne()
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
                   
            // Configure relationship with ExternalLogins
            builder.HasMany(x => x.ExternalLogins)
                   .WithOne()
                   .HasForeignKey(l => l.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Add unique constraints
            builder.HasIndex(x => x.Username).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}
