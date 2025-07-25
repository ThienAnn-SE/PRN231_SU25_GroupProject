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
    public class TestSubmissionController : ControllerBase
    {
        private readonly ITestSubmissionService _testSubmissionService;

        public TestSubmissionController(ITestSubmissionService testSubmissionService)
        {
            _testSubmissionService = testSubmissionService;
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _testSubmissionService.GetAllAsync(cancellationToken);
            if (response == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("No test submissions found."));
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return BadRequest( ApiResponse.CreateBadRequestResponse("Test submission ID is required."));
            }
            var response = await _testSubmissionService.GetByIdAsync(id, cancellationToken);
            if (response.Status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TestSubmissionDto testSubmissionDto, CancellationToken cancellationToken = default)
        {
            if (testSubmissionDto == null)
            {
                return BadRequest(ApiResponse.CreateBadRequestResponse("Test submission data is required."));
            }
            var response = await _testSubmissionService.CreateAsync(testSubmissionDto, default, cancellationToken);

            if (response.Status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }
            else if (response.Status == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
