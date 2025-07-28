using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using WebApi.Extension;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MajorController : Controller
    {
        private readonly IMajorService _majorService;
        private readonly int _recommendLimit;

        public MajorController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllMajors()
        {
            var majors = await _majorService.GetAllMajorsAsync();
            if (majors == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("No majors found."));
            }
            return Ok(majors);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMajorById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest( ApiResponse.CreateBadRequestResponse("Major ID is required."));
            }
            var major = await _majorService.GetMajorByIdAsync(id);
            if (major == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("No major found with the given ID."));
            }
            return Ok(major);
        }

        [HttpGet("Personality/{id}")]
        public async Task<IActionResult> GetMajorPersonalities(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(ApiResponse.CreateBadRequestResponse("Major ID is required."));
            }
            var personalities = await _majorService.GetMajorsByPersonalityIdAsync(id);
            return Ok(personalities);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateMajor([FromBody] CreateUpdateMajorDto majorDto)
        {
            if (majorDto == null)
            {
                return BadRequest(ApiResponse.CreateBadRequestResponse("Major data is required."));
            }
            var response = await _majorService.CreateMajorAsync(majorDto);
            if (response.Status.Equals(HttpStatusCode.BadRequest))
            {
                return BadRequest(response);
            }
            else if (response.Status.Equals(HttpStatusCode.NotFound))
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("Personality")]
        public async Task<IActionResult> AddPersonalityToMajor([FromBody] MajorPersonalityDto majorPersonalityDto)
        {
            if (majorPersonalityDto == null)
            {
                return BadRequest(ApiResponse.CreateBadRequestResponse("Major ID and personality data are required."));
            }
            var response = await _majorService.AddNewPersonalityToMajorAsync(majorPersonalityDto);
            if (response.Status.Equals(HttpStatusCode.BadRequest))
            {
                return BadRequest(response);
            }
            else if (response.Status.Equals(HttpStatusCode.NotFound))
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("Personality")]
        public async Task<IActionResult> DeletePersonalityFromMajor([FromBody] MajorPersonalityDto majorPersonalityDto)
        {
            if (majorPersonalityDto == null)
            {
                return BadRequest(ApiResponse.CreateBadRequestResponse("Major ID and personality data are required."));
            };
            var response = await _majorService.DeletePersonalityFromMajorAsync(majorPersonalityDto);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("Recommend/{personalTypeId}")]
        public async Task<IActionResult> GetRecommendMajors(Guid personalTypeId)
        {
            var response = await _majorService.GetRecommendMajorsAsync(personalTypeId, default);
            if (response.Status.Equals(HttpStatusCode.NotFound)){
                return NotFound(response);
            }else if (response.Status.Equals(HttpStatusCode.BadRequest))
            {
                return BadRequest(response);
            }
            return Ok(response);
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
