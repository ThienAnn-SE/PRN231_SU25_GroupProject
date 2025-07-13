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
        [ProducesResponseType(typeof(List<MajorDto>), 200)]
        public async Task<IActionResult> GetAllMajors(CancellationToken cancellationToken)
        {
            var majors = await _majorService.GetAllMajorsAsync(cancellationToken);
            return Ok(majors);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MajorDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMajorById(Guid id, CancellationToken cancellationToken)
        {
            var major = await _majorService.GetMajorByIdAsync(id, cancellationToken);
            if (major == null)
            {
                return NotFound();
            }
            return Ok(major);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MajorDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMajor([FromBody] CreateUpdateMajorDto majorDto, CancellationToken cancellationToken)
        {
            Guid? creatorId = null;
            var success = await _majorService.CreateMajorAsync(majorDto, creatorId, cancellationToken);
            if (!success)
            {
                ModelState.AddModelError("Name", "Major with this name might already exist or name is invalid.");
                return BadRequest(ModelState);
            }
            return CreatedAtAction(nameof(GetMajorById), new { id = majorDto.Name }, majorDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateUpdateMajorDto dto, CancellationToken cancellationToken)
        {
            var result = await _majorService.UpdateMajorAsync(id, dto);
            if (!result)
                return BadRequest("Update failed");
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var success = await _majorService.DeleteMajorAsync(id, cancellationToken);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
