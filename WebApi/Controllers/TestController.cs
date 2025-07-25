using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extension;
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

        [Authorize(Roles = Role.Admin)]
        [HttpGet()]
        public async Task<IActionResult> GetAllTests(CancellationToken cancellationToken = default)
        {
            var response = await _testService.GetAllTestsAsync(cancellationToken);
            if (response.Status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestById(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _testService.GetTestByIdAsync(id, cancellationToken);
            if (response.Status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost()]
        public async Task<IActionResult> CreateTest([FromBody] CreateTestDto testDto, CancellationToken cancellationToken = default)
        {
            if (testDto == null)
            {
                return NotFound(ApiResponse.CreateBadRequestResponse("Test data is required."));
            }
            var response = await _testService.CreateTestAsync(testDto, cancellationToken);
            if(response.Status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }else if (response.Status == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
