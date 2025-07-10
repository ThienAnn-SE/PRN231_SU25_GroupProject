using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories;
using System.Security.Cryptography;

namespace WebApi.Services
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetById (Guid id);
        Task<ApiResponse<RefreshTokenDto>> Login(LoginDto loginDto);
        Task<ApiResponse<BaseDto>> Register(RegisterDto registerDto);
    }

    public class UserService : BaseService, IUserService
    {
        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public async Task<ApiResponse<RefreshTokenDto>> Login(LoginDto loginDto)
        {
            var isExist = await unitOfWork.UserAuth.IsUserExistsAsync(loginDto.UserName);
            if (!isExist)
            {

            }

            var user = await unitOfWork.UserAuth.AuthenticateAsync(loginDto, CancellationToken.None);
            if (user == null)
            {
                return ApiResponse<RefreshTokenDto>.CreateNotFoundResponse(
                    "User not found."
                );
            }

            var refreshToken = new RefreshTokenDto()
            {
                Token = GenerateRefreshToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                UserId = user.Id,
                CreatedByIp = string.Empty
            };
            await unitOfWork.RefreshTokens.CreateAsync(refreshToken);
            return ApiResponse<RefreshTokenDto>.CreateSuccessResponse(refreshToken);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return $"{Convert.ToBase64String(randomNumber)}{Guid.NewGuid():N}";
        }

        public async Task<ApiResponse<BaseDto>> Register(RegisterDto registerDto)
        {
            var isExist = await unitOfWork.UserAuth.IsUserExistsAsync(registerDto.UserName);
            if (isExist)
            {
                return ApiResponse<BaseDto>.CreateBadRequestResponse(
                    "User already exists."
                );
            }
            var User = await unitOfWork.UserAuth.RegisterAsync(registerDto);
            return ApiResponse<BaseDto>.CreateSuccessResponse(
                new BaseDto(),
                "User registered successfully."
            );
        }

        public async Task<ApiResponse<UserDto>> GetById(Guid id)
        {
            var user = await unitOfWork.UserAuth.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResponse<UserDto>.CreateNotFoundResponse("User not found.");
            }
            return ApiResponse<UserDto>.CreateSuccessResponse(user, "User retrieved successfully.");
        }
    }
}
