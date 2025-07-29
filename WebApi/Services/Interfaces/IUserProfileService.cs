using AppCore.BaseModel;
using AppCore.Dtos;

namespace ApiService.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<ApiResponse> GetUserProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetAllUserProfile(CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateUserProfileAsync(CreateUserProfileDto userProfileDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateUserProfileAsync(UserProfileDto userProfileDto, CancellationToken cancellationToken = default);
    }
}
