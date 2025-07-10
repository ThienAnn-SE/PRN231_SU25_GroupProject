using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories;

namespace WebApi.Services
{
    public interface IUserProfileService
    {
        Task<ApiResponse> GetUserProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetAllUserProfile(CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateUserProfileAsync(UserProfileDto userProfileDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateUserProfileAsync(UserProfileDto userProfileDto, CancellationToken cancellationToken = default);
    }

    public class UserProfileService : BaseService, IUserProfileService
    {
        public UserProfileService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            // Initialize any dependencies or services here if needed
        }

        public Task<ApiResponse> CreateUserProfileAsync(UserProfileDto userProfileDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetAllUserProfile(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetUserProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> UpdateUserProfileAsync(UserProfileDto userProfileDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
