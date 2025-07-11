using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityController : ControllerBase
    {
        private readonly IUniveristyService _universityService;

        public UniversityController(IUniveristyService universityService)
        {
            _universityService = universityService;
        }

        // Placeholder for future university-related endpoints
        // This controller can be expanded with methods to handle university-related operations
        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            return Ok("UniversityController is working!");
        }
        // Additional methods for university-related operations can be added here
        
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetUniversityById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "University ID is required.");
            }
            var response = await _universityService.GetUniversityByIdAsync(id);
            if (response == null)
            {
                return ApiResponse.CreateNotFoundResponse("University not found.");
            }
            return response;
        }

        [HttpGet]
        public async Task<ApiResponse> GetAllUniversities(CancellationToken cancellationToken = default)
        {
            var response = await _universityService.GetAllUniversitiesAsync(cancellationToken);
            if (response == null)
            {
                return ApiResponse.CreateNotFoundResponse("No universities found.");
            }
            return response;
        }

        [HttpPost]
        public async Task<ApiResponse> CreateUniversityAsync([FromBody] UniversityDto universityDto, CancellationToken cancellationToken = default)
        {
            if (universityDto == null)
            {
                return ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "University data is required.");
            }
            var response = await _universityService.CreateUniversityAsync(universityDto, cancellationToken);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> UpdateUniversityAsync(Guid id, [FromBody] UniversityDto universityDto, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty || universityDto == null)
            {
                return ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "University ID and data are required.");
            }
            universityDto.Id = id; // Ensure the ID in the DTO matches the route parameter
            var response = await _universityService.UpdateUniversityAsync(universityDto, cancellationToken);
            return response;
        }

        [HttpGet("majors")]
        public async Task<ApiResponse> GetAllMajors(CancellationToken cancellationToken = default)
        {
            var response = await _universityService.GetAllMajors(cancellationToken);
            if (response == null)
            {
                return ApiResponse.CreateNotFoundResponse("No majors found.");
            }
            return response;
        }

        [HttpGet("majors/{id}")]
        public async Task<ApiResponse> GetMajorById(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "Major ID is required.");
            }
            var response = await _universityService.GetMajorById(id, cancellationToken);
            if (response == null)
            {
                return ApiResponse.CreateNotFoundResponse("Major not found.");
            }
            return response;
        }

        [HttpPost("majors")]
        public async Task<ApiResponse> CreateMajorAsync([FromBody] MajorDto majorDto, CancellationToken cancellationToken = default)
        {
            if (majorDto == null)
            {
                return ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "Major data is required.");
            }
            var response = await _universityService.CreateMajorAsync(majorDto, cancellationToken);
            return response;
        }

        [HttpPut("majors/{id}")]
        public async Task<ApiResponse> UpdateMajorByIdAsync(Guid id, [FromBody] MajorDto majorDto, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty || majorDto == null)
            {
                return ApiResponse.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "Major ID and data are required.");
            }
            majorDto.Id = id; // Ensure the ID in the DTO matches the route parameter
            var response = await _universityService.UpdateMajorByIdAsync(majorDto, cancellationToken);
            return response;
        }
    }
}
