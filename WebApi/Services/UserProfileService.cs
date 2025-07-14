using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories;

namespace WebApi.Services
{
    public interface IUserProfileService
    {
        Task<ApiResponse> GetUserProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetAllUserProfile(CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateUserProfileAsync(CreateUserProfileDto userProfileDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateUserProfileAsync(UserProfileDto userProfileDto, CancellationToken cancellationToken = default);
    }

    public class UserProfileService : BaseService, IUserProfileService
    {
        public UserProfileService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            // Initialize any dependencies or services here if needed
        }

        public async Task<ApiResponse> CreateUserProfileAsync(CreateUserProfileDto userProfileDto, CancellationToken cancellationToken = default)
        {
            if (userProfileDto == null)
            {
                return ApiResponse.CreateBadRequestResponse("User profile data cannot be null.");
            }
            if (string.IsNullOrEmpty(userProfileDto.FirstName) || string.IsNullOrEmpty(userProfileDto.LastName))
            {
                return ApiResponse.CreateBadRequestResponse("First name and last name are required.");
            }
            if (userProfileDto.DayOfBirth == default)
            {
                return ApiResponse.CreateBadRequestResponse("Date of birth is required.");
            }
            if (userProfileDto.UserAuthId == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("User ID is required.");
            }
            var existingProfile = await unitOfWork.UserProfiles.GetProfileByAuthIdAsync(userProfileDto.UserAuthId, cancellationToken);
            if (existingProfile != null)
            {
                return ApiResponse.CreateBadRequestResponse("User profile already exists for this user.");
            }
            var result = await unitOfWork.UserProfiles.CreateAsync(userProfileDto, cancellationToken);
            if (!result)
            {
                return ApiResponse.CreateInternalServerErrorResponse("Failed to create user profile.");
            }
            return ApiResponse.CreateSuccessResponse("Create new User Profile successfully");
        }

        public async Task<ApiResponse> GetAllUserProfile(CancellationToken cancellationToken = default)
        {
            var userProfiles = await unitOfWork.UserProfiles.GetAllProfilesAsync(cancellationToken);
            if (userProfiles == null || !userProfiles.Any())
            {
                return ApiResponse.CreateNotFoundResponse("No user profiles found.");
            }
            return ApiResponses<UserProfileDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "User profiles retrieved successfully.", userProfiles);
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
