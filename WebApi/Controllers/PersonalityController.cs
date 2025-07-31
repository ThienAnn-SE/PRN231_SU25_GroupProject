using ApiService.Services.Interfaces;
using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalityController : ControllerBase
    {
        private readonly IPersonalityService _personalityService;

        public PersonalityController(IPersonalityService personalityService)
        {
            _personalityService = personalityService ?? throw new ArgumentNullException(nameof(personalityService));
        }

        // Placeholder for future personality-related endpoints
        // This controller can be expanded with methods to handle personality-related operations
        [HttpGet("Test")]
        public IActionResult TestEndpoint()
        {
            return Ok("PersonalityController is working!");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonalityById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(ApiResponse<PersonalityDto>.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "Personality ID is required."));
            }
            var response = await _personalityService.GetById(id);
            if (response == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("Personality not found."));
            }
            return Ok(response);
        }

        [HttpGet("Type/{id}")]
        public async Task<IActionResult> GetPersonalityTypeById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(ApiResponse<PersonalityTypeDto>.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "Personality type ID is required."));
            }
            var response = await _personalityService.GetTypeById(id);
            if (response == null)
            {
                return NotFound(ApiResponse.CreateNotFoundResponse("Personality type not found."));
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPersonalities(CancellationToken cancellationToken = default)
        {
            var response = await _personalityService.GetAll(cancellationToken);
            if (response == null || response.Data?.Count == 0)
            {
                return NotFound(ApiResponses<PersonalityDetailDto>.CreateNotFoundResponse(
                    default,
                    "No personalities found."
                ));
            }
            return Ok(response);
        }

        [HttpGet("Type")]
        public async Task<IActionResult> GetAllPersonalityTypes(CancellationToken cancellationToken = default)
        {
            var response = await _personalityService.GetTypeAll(cancellationToken);
            if (response == null || response.Data?.Count == 0)
            {
                return NotFound( ApiResponses<PersonalityTypeDto>.CreateNotFoundResponse(
                    default,
                    "No personality types found."
                ));
            }
            return Ok(response);
        }
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateUpdatePersonalityDto dto, CancellationToken cancellationToken)
        //{
        //    var result = await _personalityService.CreateAsync(new PersonalityDto
        //    {
        //        Name = dto.Name,
        //        Description = dto.Description,
        //        PersonalityTypeId = dto.PersonalityTypeId
        //    }, creatorId: null, cancellationToken);

        //    return StatusCode((int)result.Status, result);
        //}
    }
}
