using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityController : Controller
    {
        private readonly IUniversityService _universityService;

        public UniversityController(IUniversityService universityService)
        {
            _universityService = universityService;
        }

        [HttpGet]
        public async Task<ApiResponse> GetAllUniversities(CancellationToken cancellationToken)
        {
            var universities = await _universityService.GetAllUniversitiesAsync(cancellationToken);
            if (universities == null)
            {
                return ApiResponse.CreateNotFoundResponse("No universities found.");
            }
            return universities;
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> GetUniversityById(Guid id, CancellationToken cancellationToken)
        {
            var university = await _universityService.GetUniversityByIdAsync(id, cancellationToken);
            if (university == null)
            {
                return ApiResponse.CreateNotFoundResponse("No university existed with given ID"); 
            }
            return university;
        }

        [HttpPost]
        public async Task<ApiResponse> CreateUniversity([FromBody] CreateUpdateUniversityDto universityDto, CancellationToken cancellationToken)
        {
            if (universityDto == null)
            {
                return ApiResponse.CreateBadRequestResponse("University data is required.");
            }
            var result = await _universityService.CreateUniversityAsync(universityDto);
            return result;
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> Update(Guid id, [FromBody] CreateUpdateUniversityDto dto)
        {
            if (dto == null)
            {
                return ApiResponse.CreateBadRequestResponse("University data is required.");
            }
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("University ID is required.");
            }
            var result = await _universityService.UpdateUniversityAsync(id, dto);
            if (result == null)
            {
                return ApiResponse.CreateNotFoundResponse("University does not exist with given ID.");
            }
            return result;
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
