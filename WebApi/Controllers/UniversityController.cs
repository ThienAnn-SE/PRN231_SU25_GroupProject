using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extension;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityController : Controller
    {
        private readonly IUniversityService _universityService;

        public UniversityController(IUniversityService universityService)
        {
            _universityService = universityService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllUniversities(CancellationToken cancellationToken)
        {
            var universities = await _universityService.GetAllUniversitiesAsync(cancellationToken);
            if (universities == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("No universities found."));
            }
            return Ok(universities);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUniversityById(Guid id, CancellationToken cancellationToken)
        {
            var university = await _universityService.GetUniversityByIdAsync(id, cancellationToken);
            if (university == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("No university existed with given ID")); 
            }
            return Ok(university);
        }

        [Authorize(Roles = Role.AdminAndManager)]
        [HttpPost]
        public async Task<IActionResult> CreateUniversity([FromBody] CreateUpdateUniversityDto universityDto, CancellationToken cancellationToken)
        {
            if (universityDto == null)
            {
                return BadRequest(ApiResponse.CreateBadRequestResponse("University data is required."));
            }
            var result = await _universityService.CreateUniversityAsync(universityDto);
            if (result.Status == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = Role.AdminAndManager)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateUpdateUniversityDto dto)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse.CreateBadRequestResponse("University data is required."));
            }
            if (id == Guid.Empty)
            {
                return BadRequest(ApiResponse.CreateBadRequestResponse("University ID is required."));
            }
            var result = await _universityService.UpdateUniversityAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("University does not exist with given ID."));
            }
            return Ok(result);
        }

        //[HttpDelete("{id}")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> DeleteUniversity(Guid id, CancellationToken cancellationToken)
        //{
        //    var success = await _universityService.DeleteUniversityAsync(id, cancellationToken);
        //    if (!success)
        //    {
        //        return NotFound(); 
        //    }
        //    return NoContent(); 
        //}
    }
}
