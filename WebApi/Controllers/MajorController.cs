using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MajorController : Controller
    {
        private readonly IMajorService _majorService;

        public MajorController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        [HttpGet]
        public async Task<ApiResponse> GetAllMajors()
        {
            var majors = await _majorService.GetAllMajorsAsync();
            if (majors == null)
            {
                return ApiResponse.CreateNotFoundResponse("No majors found.");
            }
            return majors;
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> GetMajorById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Major ID is required.");
            }
            var major = await _majorService.GetMajorByIdAsync(id);
            if (major == null)
            {
                return ApiResponse.CreateNotFoundResponse("No major found with the given ID.");
            }
            return major;
        }

        [HttpPost]
        public async Task<ApiResponse> CreateMajor([FromBody] CreateUpdateMajorDto majorDto)
        {
            if (majorDto == null)
            {
                return ApiResponse.CreateBadRequestResponse("Major data is required.");
            }
            return await _majorService.CreateMajorAsync(majorDto);
        }

        [HttpGet("Personality/{id}")]
        public async Task<ApiResponse> GetMajorPersonalities(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Major ID is required.");
            }
            var personalities = await _majorService.GetMajorsByPersonalityIdAsync(id);
            return personalities;
        }


        [HttpPost("Personality")]
        public async Task<ApiResponse> AddPersonalityToMajor([FromBody] MajorPersonalityDto majorPersonalityDto)
        {
            if (majorPersonalityDto == null)
            {
                return ApiResponse.CreateBadRequestResponse("Major ID and personality data are required.");
            }
            return await _majorService.AddNewPersonalityToMajorAsync(majorPersonalityDto);
        }

        [HttpDelete("Personality")]
        public async Task<ApiResponse> DeletePersonalityFromMajor([FromBody] MajorPersonalityDto majorPersonalityDto)
        {
            if (majorPersonalityDto == null)
            {
                return ApiResponse.CreateBadRequestResponse("Major ID and personality data are required.");
            }
            return await _majorService.DeletePersonalityFromMajorAsync(majorPersonalityDto);
        }

        //[HttpPut("{id}")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> Update(Guid id, [FromBody] CreateUpdateMajorDto dto, CancellationToken cancellationToken)
        //{
        //    var result = await _majorService.UpdateMajorAsync(id, dto);
        //    if (!result)
        //        return BadRequest("Update failed");
        //    return Ok("Updated successfully");
        //}

        //[HttpDelete("{id}")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        //{
        //    var success = await _majorService.DeleteMajorAsync(id, cancellationToken);
        //    if (!success)
        //    {
        //        return NotFound();
        //    }
        //    return NoContent();
        //}
    }
}
