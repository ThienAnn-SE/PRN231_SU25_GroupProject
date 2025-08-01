using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Extensions;
using Repositories.Interfaces;
using System.Linq.Expressions;

namespace Repositories
{
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

            var isValidPassword = PasswordHasher.VerifyPassword(loginDto.Password, entity.Salt, entity.PasswordHash);

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
                await _repository.UpdateAsync(entity, entity.Id, cancellationToken);
                return null;
            }

            entity.ResetAccessFailedCount();
            await _repository.UpdateAsync(entity, entity.Id, cancellationToken);

            return new UserDto()
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                EmailConfirmed = entity.EmailConfirmed,
                TwoFactorEnabled = entity.TwoFactorEnabled,
                LockoutEnd = entity.LockoutEnd,
                AccessFailedCount = entity.AccessFailedCount,
                Role = entity.Role
            };
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var salt = GenerateSalt();
            var hashedPassword = PasswordHasher.HashPassword(registerDto.Password, salt);
            return await _repository.SaveAsync(new User(registerDto.UserName, registerDto.Email, hashedPassword, salt), default, default);
        }

        public async Task<bool> InitTestUser()
        {
            var salt1 = GenerateSalt();
            var salt2 = GenerateSalt();
            var salt3 = GenerateSalt();
            var hashedPassword1 = PasswordHasher.HashPassword("@1", salt1);
            var hashedPassword2 = PasswordHasher.HashPassword("@1", salt2);
            var hashedPassword3 = PasswordHasher.HashPassword("@1", salt3);
            var admin = new User("Admin", "admin@gmail.com", hashedPassword1, salt1) {
                Id = Guid.NewGuid(),
                Role = UserRole.Admin
            };
            var manager = new User("Manager", "manager@gmail.com", hashedPassword2, salt2) {
                Id = Guid.NewGuid(),
                Role = UserRole.Manager
            };
            var user = new User("user", "user@gmail.com", hashedPassword3, salt3) {
                Id = Guid.NewGuid() 
            };
            var result1 = await _repository.SaveAsync(admin, default, default);
            var result2 = await _repository.SaveAsync(manager, default, default);
            var result3 =await _repository.SaveAsync(user, default, default);
            return result1 && result2 && result3;
        }

        private static string GenerateSalt()
        {
            return Guid.NewGuid().ToString("N");
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

        public async Task<UserDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<User, bool>>[]
            {
                x => x.Username.Equals(username)
            };
            var entity = await _repository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return null;
            }
            return new UserDto()
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                EmailConfirmed = entity.EmailConfirmed,
                TwoFactorEnabled = entity.TwoFactorEnabled,
                LockoutEnd = entity.LockoutEnd,
                AccessFailedCount = entity.AccessFailedCount,
                Role = entity.Role
            };
        }

        public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filters = new Expression<Func<User, bool>>[]
            {
                    x => x.Id == id
            };
            var user = await _repository.FindOneAsync(filters, cancellationToken: cancellationToken);
            if (user == null)
            {
                return null;
            }
            return new UserDto()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                Role = user.Role
            };
        }

        public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetAllAsync(cancellationToken: cancellationToken);
            var userDtos = user.Select(u => new UserDto()
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                TwoFactorEnabled = u.TwoFactorEnabled,
                LockoutEnd = u.LockoutEnd,
                AccessFailedCount = u.AccessFailedCount,
                Role = u.Role
            }).ToList();
            return userDtos;
        }

        public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<User, bool>>[]
            {
                x => x.Email.Equals(email)
            };
            var entity = await _repository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return null;
            }
            return new UserDto()
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                EmailConfirmed = entity.EmailConfirmed,
                TwoFactorEnabled = entity.TwoFactorEnabled,
                LockoutEnd = entity.LockoutEnd,
                AccessFailedCount = entity.AccessFailedCount,
                Role = entity.Role
            };
        }
    }
}
