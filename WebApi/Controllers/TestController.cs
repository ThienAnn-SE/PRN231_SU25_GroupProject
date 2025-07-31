using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet()]
        public async Task<ApiResponse> GetAllTests(CancellationToken cancellationToken = default)
        {
            var response = await _testService.GetAllTestsAsync(cancellationToken);
            return response;
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> GetTestById(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Test ID is required.");
            }
            var response = await _testService.GetTestByIdAsync(id, cancellationToken);
            return response;
        }

        [HttpPost()]
        public async Task<ApiResponse> CreateTest([FromBody] CreateTestDto testDto, CancellationToken cancellationToken = default)
        {
            if (testDto == null)
            {
                return ApiResponse.CreateBadRequestResponse("Test data is required.");
            }
            var response = await _testService.CreateTestAsync(testDto, cancellationToken);
            return response;
        }
        [HttpGet("by-type/{type}")]
        public async Task<ApiResponse> GetByPersonalityType(string type, CancellationToken cancellationToken = default)
        {
            var response = await _testService.GetTestByPersonalityTypeAsync(type, cancellationToken);
            return response;
        }
    }
}
