using AppCore.Dtos;

namespace Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellation = default);
        Task<UserProfileDto?> GetProfileByAuthIdAsync(Guid userAuthId, CancellationToken cancellation = default);
        Task<List<UserProfileDto>> GetAllProfilesAsync(CancellationToken cancellation = default);
        Task<bool> CreateAsync(CreateUserProfileDto profileDto, CancellationToken cancellation = default);
        Task<bool> UpdateProfileAsync(UserProfileDto profile);
    }
}
