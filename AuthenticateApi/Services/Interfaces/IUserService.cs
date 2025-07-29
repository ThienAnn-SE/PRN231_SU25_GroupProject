using AppCore.BaseModel;
using AppCore.Dtos;

namespace ApiAuthentication.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetById(Guid id);
        Task<ApiResponse> Login(LoginDto loginDto, JwtOptions jwtOptions);
        Task<ApiResponse> Register(RegisterDto registerDto);
        Task<ApiResponse> InitTestUsers();
        Task<ApiResponse> ValidateToken(JwtOptions jwtOptions);
        Task<ApiResponse> KeepAlive(JwtOptions jwtOptions);
    }
}
