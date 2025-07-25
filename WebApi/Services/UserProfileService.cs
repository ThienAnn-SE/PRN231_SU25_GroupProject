using AppCore.BaseModel;
using AppCore.Dtos;
using AppCore.Entities;
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

        public async Task<ApiResponse> GetUserProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var userProfile = await unitOfWork.UserProfiles.GetProfileByAuthIdAsync(userId, cancellationToken);
            if (userProfile == null)
            {
                return ApiResponse.CreateNotFoundResponse("User profile does not exist with given AuthId");
            }
            return ApiResponse<UserDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "User profile retrived successfully");
        }

        public async Task<ApiResponse> UpdateUserProfileAsync(UserProfileDto userProfileDto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userProfileDto.FirstName) || string.IsNullOrEmpty(userProfileDto.LastName))
            {
                return ApiResponse.CreateBadRequestResponse("First name and last name are required.");
            }
            if (string.IsNullOrEmpty(userProfileDto.Address))
            {
                return ApiResponse.CreateBadRequestResponse("Address are required.");
            }
            if (userProfileDto.UserAuthId == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("User ID is required.");
            }
            if (userProfileDto.DayOfBirth == default)
            {
                return ApiResponse.CreateBadRequestResponse("Date of birth is required.");
            }
            var now = DateTime.Now;
            var ageThreshold = now.AddYears(-10);
            if (userProfileDto.DayOfBirth > ageThreshold)
            {
                return ApiResponse.CreateBadRequestResponse("Date of birth is invalid. Must be at least 10 years old.");
            }

            var userProfile = await unitOfWork.UserProfiles.GetProfileByAuthIdAsync(userProfileDto.UserAuthId, cancellationToken);
            if (userProfile == null)
            {
                return ApiResponse.CreateNotFoundResponse("User profile does not exist with given AuthId");
            }
            if (!userProfileDto.FirstName.Equals(userProfile.FirstName))
            {
                userProfile.FirstName = userProfileDto.FirstName;
            }
            if (!userProfileDto.LastName.Equals(userProfile.LastName))
            {
                userProfile.LastName = userProfileDto.LastName;
            }
            if (!userProfileDto.Address.Equals(userProfile.Address))
            {
                userProfile.Address = userProfileDto.Address;
            }
            if (!userProfileDto.Gender.Equals(userProfile.Gender))
            {
                userProfile.Gender = userProfileDto.Gender;
            }
            if (!userProfileDto.Gender.Equals(userProfile.Gender))
            {
                userProfile.DayOfBirth = userProfileDto.DayOfBirth;
            }
            if (!string.IsNullOrEmpty(userProfileDto.ProfilePictureUrl) && !userProfileDto.ProfilePictureUrl.Equals(userProfile.ProfilePictureUrl))
            {
                userProfile.ProfilePictureUrl = userProfileDto.ProfilePictureUrl;
            }

            var result = await unitOfWork.UserProfiles.UpdateProfileAsync(userProfile);
            if (!result)
            {
                return ApiResponse.CreateNotFoundResponse("Error when updating profile");
            }
            return ApiResponse.CreateSuccessResponse("Update user profile successfully");
        }
    }
}
