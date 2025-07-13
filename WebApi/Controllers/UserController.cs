using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Services;


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
                ApiResponse.CreateBadRequestResponse("Login data is required.");
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
    }
}
