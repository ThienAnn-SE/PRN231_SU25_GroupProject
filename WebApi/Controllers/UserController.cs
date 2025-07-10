using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;


namespace WebApi.Controllers
{
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
        public async Task<ApiResponse<UserDto>> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse<UserDto>.CreateBadRequestResponse("User ID is required.");
            }
            var response = await _userService.GetById(id);
            return response;
        }

        [HttpPost("login")]
        public async Task<ApiResponse<RefreshTokenDto>> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                ApiResponse<RefreshTokenDto>.CreateBadRequestResponse("Login data is required.");
            }
            var response = await _userService.Login(loginDto);
            return response;
        }

        [HttpPost("register")]
        public async Task<ApiResponse<BaseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                ApiResponse<BaseDto>.CreateBadRequestResponse("Registration data is required.");
            }
            var response = await _userService.Register(registerDto);
            return response;
        }
    }
}
