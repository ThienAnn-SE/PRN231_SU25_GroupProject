using ApiService.Services.Interfaces;
using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extension;

namespace ApiService.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetUserProfileById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(ApiResponse<PersonalityDto>.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "User Profile ID is required."));
            }
            var response = await _userProfileService.GetUserProfileByIdAsync(id);
            if (response == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("User Profile not found."));
            }
            return Ok(response);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllUserProfiles(CancellationToken cancellationToken = default)
        {
            var response = await _userProfileService.GetAllUserProfile(cancellationToken);
            if (response == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("No user profiles found."));
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserProfileAsync([FromBody] CreateUserProfileDto createUserProfileDto, CancellationToken cancellationToken = default)
        {
            if (createUserProfileDto == null)
            {
                return BadRequest(ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "User Profile data is required."));
            }
            var response = await _userProfileService.CreateUserProfileAsync(createUserProfileDto, cancellationToken);
            if (response.Status == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(response);
            }
            else if (response.Status == System.Net.HttpStatusCode.InternalServerError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfileAsync(Guid id, [FromBody] UserProfileDto userProfileDto, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty || userProfileDto == null)
            {
                return BadRequest(ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "User Profile ID and data are required."));
            }
            userProfileDto.Id = id; // Ensure the ID in the DTO matches the route parameter
            var response = await _userProfileService.UpdateUserProfileAsync(userProfileDto, cancellationToken);
            if (response.Status == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(response);
            }
            else if (response.Status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}