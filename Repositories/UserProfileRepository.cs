using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellation = default);
        Task<bool> UpdateProfileAsync(UserProfileDto profile);
    }

    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly CrudRepository<UserProfile> _repository;

        public UserProfileRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _repository = new CrudRepository<UserProfile>(dbContext, transaction);
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