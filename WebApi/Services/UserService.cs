using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories;
using WebApi.Extension;

namespace WebApi.Services
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetById (Guid id);
        Task<ApiResponse> Login(LoginDto loginDto, JwtOptions jwtOptions);
        Task<ApiResponse> Register(RegisterDto registerDto);
        Task<ApiResponse> InitTestUsers();
    }

    public class UserService : BaseService, IUserService
    {
        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public async Task<ApiResponse> Login(LoginDto loginDto, JwtOptions jwtOptions)
        {
            var isExist = await unitOfWork.UserAuth.IsUserExistsAsync(loginDto.UserName);
            if (!isExist)
            {
                return ApiResponse.CreateNotFoundResponse(
                    "User not found."
                );
            }

            var user = await unitOfWork.UserAuth.AuthenticateAsync(loginDto, CancellationToken.None);
            if (user == null)
            {
                return ApiResponse<RefreshTokenDto>.CreateNotFoundResponse(
                    default,
                    "User not found."
                );
            }

            var refreshToken = new RefreshTokenDto()
            {
                Token = JwtExtensions.GenerateAccessToken(user, jwtOptions),
                ExpiryDate = DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpiryMinutes),
                UserId = user.Id,
                CreatedByIp = string.Empty
            };
            var isSuccess = await unitOfWork.RefreshTokens.CreateAsync(refreshToken, user.Id, CancellationToken.None);
            if (!isSuccess)
            {
                return ApiResponse.CreateInternalServerErrorResponse(
                    "Failed to create refresh token."
                );
            }
            return ApiResponse<RefreshTokenDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Login success" ,refreshToken);
        }

        public async Task<ApiResponse> Register(RegisterDto registerDto)
        {
            var isExist = await unitOfWork.UserAuth.IsUserExistsAsync(registerDto.UserName);
            if (isExist)
            {
                return ApiResponse.CreateBadRequestResponse(
                    "User already exists."
                );
            }
            var User = await unitOfWork.UserAuth.RegisterAsync(registerDto);
            return ApiResponse.CreateSuccessResponse(
                "User registered successfully."
            );
        }

        public async Task<ApiResponse<UserDto>> GetById(Guid id)
        {
            var user = await unitOfWork.UserAuth.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<UserDto>.CreateNotFoundResponse(default, "User not found.");
            }
            return ApiResponse<UserDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "User retrieved successfully.", user);
        }

        public async Task<ApiResponse> InitTestUsers()
        {
            var result = await unitOfWork.UserAuth.InitTestUser();
            if (!result)
            {
                return ApiResponse.CreateBadRequestResponse("Failt");
            }
            return ApiResponse.CreateSuccessResponse();
        }
    }
}
