using AppCore;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;

namespace Repositories
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

    /// <summary>
    /// Implementation of the Unit of Work pattern that coordinates multiple repositories
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDbTransaction _transaction;
        private bool _disposed;

        private IRefreshTokenRepository _refreshTokenRepository = null!;
        private IExternalLoginRepository _externalLoginRepository = null!;
        private IUserRepository _userAuthRepository = null!;
        private IUserProfileRepository _userProfileRepository = null!;
        private IUniversityRepository _universityRepository = null!;
        private ITestSubmissionRepository _testSubmissionRepository = null!;
        private ITestRepository _testRepoisory = null!;
        private IPersonalityRepository _personalityRepository = null!;
        private IMajorRepository _majorRepository = null!;

        /// <summary>
        /// Initializes a new instance of the Unit of Work
        /// </summary>
        public UnitOfWork(
            ApplicationDbContext dbContext, 
            IDbTransaction transaction)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        /// <summary>
        /// Repository for managing refresh tokens
        /// </summary>
        public IRefreshTokenRepository RefreshTokens =>
            _refreshTokenRepository ??= new RefreshTokenRepository(_dbContext, _transaction);

        /// <summary>
        /// Repository for managing external logins
        /// </summary>
        public IExternalLoginRepository ExternalLogins =>
            _externalLoginRepository ??= new ExternalLoginRepository(_dbContext, _transaction);

        public IUserRepository UserAuth =>
            _userAuthRepository ??= new UserRepository(_dbContext, _transaction);

        public IUserProfileRepository UserProfiles =>
            _userProfileRepository ??= new UserProfileRepository(_dbContext, _transaction);

        public IUniversityRepository UniversityRepository =>
            _universityRepository ??= new UniversityRepository(_dbContext, _transaction);

        public ITestSubmissionRepository TestSubmissionRepository =>
            _testSubmissionRepository ??= new TestSubmissionRepository(_dbContext, _transaction);

        public ITestRepository TestRepository =>
            _testRepoisory ??= new TestRepository(_dbContext, _transaction);

        public IPersonalityRepository PersonalityRepository =>
            _personalityRepository ??= new PersonalityRepository(_dbContext, _transaction);

        public IMajorRepository MajorRepository =>
            _majorRepository ??= new MajorRepository(_dbContext, _transaction);

        /// <summary>
        /// Disposes the context
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the context
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;
        }
    }
}