namespace Repositories.Interfaces
{
    /// <summary>
    /// Coordinates the work of multiple repositories by providing a single transaction scope
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Repository for managing refresh tokens
        /// </summary>
        IRefreshTokenRepository RefreshTokens { get; }

        /// <summary>
        /// Repository for managing external logins
        /// </summary>
        IExternalLoginRepository ExternalLogins { get; }

        IUserRepository UserAuth { get; }

        IUserProfileRepository UserProfiles { get; }
        IUniversityRepository UniversityRepository { get; }
        ITestSubmissionRepository TestSubmissionRepository { get; }
        ITestRepository TestRepository { get; }
        IPersonalityRepository PersonalityRepository { get; }
        IMajorRepository MajorRepository { get; }
    }
}
