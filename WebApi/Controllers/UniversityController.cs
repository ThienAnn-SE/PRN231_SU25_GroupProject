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
        [ProducesResponseType(typeof(List<UniversityDto>), 200)]
        public async Task<IActionResult> GetAllUniversities(CancellationToken cancellationToken)
        {
            var universities = await _universityService.GetAllUniversitiesAsync(cancellationToken);
            return Ok(universities);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UniversityDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUniversityById(Guid id, CancellationToken cancellationToken)
        {
            var university = await _universityService.GetUniversityByIdAsync(id, cancellationToken);
            if (university == null)
            {
                return NotFound(); 
            }
            return Ok(university);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UniversityDto), 201)] 
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUniversity([FromBody] UniversityDto universityDto, CancellationToken cancellationToken)
        {
            Guid? creatorId = null; 

            var success = await _universityService.CreateUniversityAsync(universityDto, creatorId, cancellationToken);
            if (!success)
            {
                ModelState.AddModelError("Name", "University with this name might already exist or name is invalid.");
                return BadRequest(ModelState); 
            }
            return CreatedAtAction(nameof(GetUniversityById), new { id = universityDto.Id }, universityDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUniversity(Guid id, [FromBody] UniversityDto universityDto, CancellationToken cancellationToken)
        {
            if (id != universityDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            Guid? updaterId = null; 

            var success = await _universityService.UpdateUniversityAsync(universityDto, updaterId, cancellationToken);
            if (!success)
            {
                var existingUniversity = await _universityService.GetUniversityByIdAsync(id, cancellationToken);
                if (existingUniversity == null)
                {
                    return NotFound();
                }
                ModelState.AddModelError("UpdateError", "Failed to update university. Check input data.");
                return BadRequest(ModelState); 
            }
            return NoContent(); 
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUniversity(Guid id, CancellationToken cancellationToken)
        {
            var success = await _universityService.DeleteUniversityAsync(id, cancellationToken);
            if (!success)
            {
                return NotFound(); 
            }
            return NoContent(); 
        }
    }
}
