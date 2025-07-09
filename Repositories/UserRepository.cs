using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Extensions;
using System.Linq.Expressions;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<UserDto?> AuthenticateAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<bool> IsUserExistsAsync(string username);
    }

    public class UserRepository : IUserRepository
    {
        private const int MaxFailedAccessCount = 5;
        private const int LockoutDurationMinutes = 15;

        private readonly CrudRepository<User> _repository;

        public UserRepository(DbContext dbContext,
            IDbTransaction transaction)
        {
            _repository = new CrudRepository<User>(dbContext, transaction);
        }

        public async Task<UserDto?> AuthenticateAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<User, bool>>[]
            {
                x => x.Email == loginDto.Email
            };
            var entity = await _repository
                .FindOneAsync(filter, cancellationToken: cancellationToken);

            if (entity == null)
            {
                return null;
            }

            var isValidPassword = PasswordHasher.VerifyPassword(loginDto.Password, entity.PasswordHash, entity.Salt);

            var isExceedingMaxFailedAccessCount = entity.AccessFailedCount >= MaxFailedAccessCount;
            var isLockedOut = entity.LockoutEnd.HasValue && entity.LockoutEnd > DateTime.UtcNow;
            // Check if the user is locked out or has exceeded the maximum failed access count
            if (isExceedingMaxFailedAccessCount || isLockedOut)
            {
                // User is locked out due to too many failed attempts
                return null;
            }

            if (!isValidPassword)
            {
                entity.RecordFailedAccess(MaxFailedAccessCount, TimeSpan.FromMinutes(LockoutDurationMinutes));
                await _repository.SaveAsync(entity, entity.Id, cancellationToken);
                return null;
            }

            entity.ResetAccessFailedCount();
            await _repository.SaveAsync(entity, entity.Id, cancellationToken);

            return new UserDto()
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                EmailConfirmed = entity.EmailConfirmed,
                TwoFactorEnabled = entity.TwoFactorEnabled,
                LockoutEnd = entity.LockoutEnd,
                AccessFailedCount = entity.AccessFailedCount
            };
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            return await _repository.SaveAsync(new User(registerDto.UserName, registerDto.Email, registerDto.PasswordHash, registerDto.Salt), default, default);
        }

        public async Task<bool> IsUserExistsAsync(string username)
        {
            var filter = new Expression<Func<User, bool>>[]
            {
                x => x.Username == username
            };
            var entity = await _repository.FindOneAsync(filter, cancellationToken: default);
            return entity != null;
        }
    }
}
