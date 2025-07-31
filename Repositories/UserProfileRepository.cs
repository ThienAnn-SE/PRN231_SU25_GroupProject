using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Linq.Expressions;

namespace Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly CrudRepository<UserProfile> _repository;

        public UserProfileRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _repository = new CrudRepository<UserProfile>(dbContext, transaction);
        }

        public async Task<bool> CreateAsync(CreateUserProfileDto profileDto, CancellationToken cancellation = default)
        {
            var newProfile = new UserProfile
            {
                Id = Guid.NewGuid(),
                FirstName = profileDto.FirstName,
                LastName = profileDto.LastName,
                DayOfBirth = profileDto.DayOfBirth,
                UserAuthId = profileDto.UserAuthId,
                Gender = profileDto.Gender,
                Address = profileDto.Address,
                ProfilePictureUrl = profileDto.ProfilePictureUrl
            };
            return await _repository.SaveAsync(newProfile, profileDto.UserAuthId, cancellation);
        }

        public async Task<List<UserProfileDto>> GetAllProfilesAsync(CancellationToken cancellation = default)
        {
            var filter = new Expression<Func<UserProfile, bool>>[]
            {
                x => x.DeletedAt == null // Assuming DeletedAt is a DateTime field indicating soft deletion
            };
            var profiles = await _repository.FindAsync(filter);
            if (profiles == null || !profiles.Any())
            {
                return new List<UserProfileDto>();
            }
            return profiles.Select(p => new UserProfileDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DayOfBirth = p.DayOfBirth,
                UserAuthId = p.UserAuthId,
                Gender = p.Gender,
                Address = p.Address,
                ProfilePictureUrl = p.ProfilePictureUrl
            }).ToList();
        }

        public async Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellation = default)
        {
            var entity = await _repository.FindByIdAsync(userId, cancellation);

            if (entity == null) {
                return null;
            }

            return new UserProfileDto()
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DayOfBirth = entity.DayOfBirth,
                Gender = entity.Gender,
                Address = entity.Address,
                ProfilePictureUrl = entity.ProfilePictureUrl,
                UserAuthId = entity.UserAuthId
            };
        }

        public async Task<UserProfileDto?> GetProfileByAuthIdAsync(Guid userAuthId, CancellationToken cancellation = default)
        {
            if (userAuthId == Guid.Empty)
            {
                return null;
            }
            var filter = new Expression<Func<UserProfile, bool>>[]
            {
                x => x.UserAuthId == userAuthId
            };
            var entity = await _repository.FindOneAsync(filter);
            if (entity == null)
            {
                return null;
            }
            return new UserProfileDto()
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DayOfBirth = entity.DayOfBirth,
                Gender = entity.Gender,
                Address = entity.Address,
                ProfilePictureUrl = entity.ProfilePictureUrl,
                UserAuthId = entity.UserAuthId
                };
        }

        public async Task<bool> UpdateProfileAsync(UserProfileDto profile)
        {
            var existingProfile = await _repository.FindByIdAsync(profile.Id, default);


            if (existingProfile == null)
                return false;

            existingProfile.FirstName = profile.FirstName;
            existingProfile.LastName = profile.LastName;
            existingProfile.DayOfBirth = profile.DayOfBirth;
            existingProfile.Gender = profile.Gender;
            existingProfile.Address = profile.Address;
            existingProfile.ProfilePictureUrl = profile.ProfilePictureUrl;

            return await _repository.SaveAsync(existingProfile, profile.CreatorId, default);
        }
    }
}