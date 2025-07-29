using ApiAuthentication.Services.Interfaces;
using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet("Init")]
        public async Task<IActionResult> InitTestUsers()
        {
            await _userService.InitTestUsers(); ;
            return Ok();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse<UserDto>.CreateResponse(System.Net.HttpStatusCode.BadRequest, false, "User ID is required.");
            }
            var response = await _userService.GetById(id);
            return response;
        }

        [HttpPost("login")]
        public async Task<ApiResponse> Login([FromBody] LoginDto loginDto, [FromServices] IOptions<JwtOptions> jwtOptions)
        {
            if (loginDto == null)
            {
                return ApiResponse.CreateBadRequestResponse("Login data is required.");
            }
            var response = await _userService.Login(loginDto, jwtOptions.Value);
            return response;
        }

        [HttpPost("register")]
        public async Task<ApiResponse> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                ApiResponse.CreateBadRequestResponse("Registration data is required.");
            }
            var response = await _userService.Register(registerDto);
            return response;
        }

        [HttpGet("Validate-token")]
        public async Task<IActionResult> ValidateToken([FromServices] IOptions<JwtOptions> jwtOptions)
        {
            var response = await _userService.ValidateToken(jwtOptions.Value);
            if (response.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized(response);
            }
            else if (response.Status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("/Keep-alive")]
        public async Task<IActionResult> KeepAliveToken([FromServices] IOptions<JwtOptions> jwtOptions)
        {
            var result = await _userService.KeepAlive(jwtOptions.Value);
            if (result.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized(result);
            }else if (result.Status == System.Net.HttpStatusCode.NotFound)
            {
                NotFound(result);
            }
            return Ok(result);
        }
    }
}
