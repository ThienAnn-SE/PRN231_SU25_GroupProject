using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        // Placeholder for future user profile-related endpoints
        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            return Ok("UserProfileController is working!");
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> GetUserProfileById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse<PersonalityDto>.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "User Profile ID is required.");
            }
            var response = await _userProfileService.GetUserProfileByIdAsync(id);
            if (response == null)
            {
                return ApiResponse.CreateNotFoundResponse("User Profile not found.");
            }
            return response;
        }

        [HttpGet]
        public async Task<ApiResponse> GetAllUserProfiles(CancellationToken cancellationToken = default)
        {
            var response = await _userProfileService.GetAllUserProfile(cancellationToken);
            if (response == null)
            {
                return ApiResponse.CreateNotFoundResponse("No user profiles found.");
            }
            return response;
        }

        [HttpPost]
        public async Task<ApiResponse> CreateUserProfileAsync([FromBody] UserProfileDto userProfileDto, CancellationToken cancellationToken = default)
        {
            if (userProfileDto == null)
            {
                return ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "User Profile data is required.");
            }
            var response = await _userProfileService.CreateUserProfileAsync(userProfileDto, cancellationToken);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> UpdateUserProfileAsync(Guid id, [FromBody] UserProfileDto userProfileDto, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty || userProfileDto == null)
            {
                return ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "User Profile ID and data are required.");
            }
            userProfileDto.Id = id; // Ensure the ID in the DTO matches the route parameter
            var response = await _userProfileService.UpdateUserProfileAsync(userProfileDto, cancellationToken);
            return response;
        }
    }
}
