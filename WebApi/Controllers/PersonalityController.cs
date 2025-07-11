using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalityController : ControllerBase
    {
        private readonly IPersonalityService _personalityService;

        public PersonalityController(IPersonalityService personalityService)
        {
            _personalityService = personalityService ?? throw new ArgumentNullException(nameof(personalityService));
        }

        // Placeholder for future personality-related endpoints
        // This controller can be expanded with methods to handle personality-related operations
        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            return Ok("PersonalityController is working!");
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> GetPersonalityById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse<PersonalityDto>.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "Personality ID is required.");
            }
            var response = await _personalityService.GetById(id);
            if (response == null)
            {
                return ApiResponse.CreateNotFoundResponse("Personality not found.");
            }
            return response;
        }

        [HttpGet("type/{id}")]
        public async Task<ApiResponse> GetPersonalityTypeById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse<PersonalityTypeDto>.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "Personality type ID is required.");
            }
            var response = await _personalityService.GetTypeById(id);
            if (response == null)
            {
                return ApiResponse.CreateNotFoundResponse("Personality type not found.");
            }
            return response;
        }

        [HttpGet]
        public async Task<ApiResponses<PersonalityDetailDto>> GetAllPersonalities(CancellationToken cancellationToken = default)
        {
            var response = await _personalityService.GetAll(cancellationToken);
            if (response == null || response.Data?.Count == 0)
            {
                return ApiResponses<PersonalityDetailDto>.CreateNotFoundResponse(
                    default,
                    "No personalities found."
                );
            }
            return response;
        }

        [HttpGet("types")]
        public async Task<ApiResponses<PersonalityTypeDto>> GetAllPersonalityTypes(CancellationToken cancellationToken = default)
        {
            var response = await _personalityService.GetTypeAll(cancellationToken);
            if (response == null || response.Data?.Count == 0)
            {
                return ApiResponses<PersonalityTypeDto>.CreateNotFoundResponse(
                    default,
                    "No personality types found."
                );
            }
            return response;
        }
    }
}
