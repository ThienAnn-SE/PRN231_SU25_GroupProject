using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestSubmissionController : ControllerBase
    {
        private readonly ITestSubmissionService _testSubmissionService;

        public TestSubmissionController(ITestSubmissionService testSubmissionService)
        {
            _testSubmissionService = testSubmissionService;
        }

        [HttpGet]
        public async Task<ApiResponse> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _testSubmissionService.GetAllAsync(cancellationToken);
            if (response == null)
            {
                return ApiResponse.CreateNotFoundResponse("No test submissions found.");
            }
            return response;
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Test submission ID is required.");
            }
            var response = await _testSubmissionService.GetByIdAsync(id, cancellationToken);
            return response;
        }

        [HttpPost]
        public async Task<ApiResponse> CreateAsync([FromBody] TestSubmissionDto testSubmissionDto, CancellationToken cancellationToken = default)
        {
            if (testSubmissionDto == null)
            {
                return ApiResponse.CreateBadRequestResponse("Test submission data is required.");
            }
            var response = await _testSubmissionService.CreateAsync(testSubmissionDto, default, cancellationToken);
            return response;
        }
    }
}
