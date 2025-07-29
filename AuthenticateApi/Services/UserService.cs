using ApiAuthentication.Services.Interfaces;
using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using System.Security.Claims;
using WebApi.Extension;

namespace ApiAuthentication.Services
{

    public class UserService : BaseService, IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse> Login(LoginDto loginDto, JwtOptions jwtOptions)
        {
            var exstingUser = await unitOfWork.UserAuth.GetByEmailAsync(loginDto.Email);
            if (exstingUser == null)
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

        public async Task<ApiResponse> ValidateToken(JwtOptions jwtOptions)
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.
                Headers.
                Authorization.
                FirstOrDefault()?.Split(" ").
                Last();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return ApiResponse.CreateUnauthorizedResponse("Unauthorized - Empty token");
            }

            ClaimsPrincipal? principal;
            try
            {
                principal = JwtExtensions.ValidateToken(accessToken, jwtOptions);
            }
            catch (SecurityTokenException)
            {
                return ApiResponse.CreateUnauthorizedResponse("Unauthorized - Invalid ClaimsPrincipal");
            }

            var accountIdString = principal.FindFirst(AppClaimTypes.Id)?.Value;
            if (!Guid.TryParse(accountIdString, out var accountId) || accountId == Guid.Empty)
            {
                return ApiResponse.CreateUnauthorizedResponse("Unauthorized - Invalid Account ID");
            }

            var now = DateTime.UtcNow;

            var user = await unitOfWork.UserAuth.GetByIdAsync(accountId);
            if (user == null)
            {
                return ApiResponse.CreateNotFoundResponse("Unauthorized - Account ID does not exist");
            }

            var tokens = await unitOfWork.RefreshTokens.GetActiveTokensByUserIdAsync(accountId);

            var token = tokens.FirstOrDefault(t =>
                t.Token.Equals(accessToken) &&
                t.UserId == accountId &&
                t.ExpiryDate > now);

            if (token == null)
            {
                return ApiResponse.CreateNotFoundResponse("Unauthorized - Token does not exist");
            }
            if (!token.IsActive)
            {
                return ApiResponse.CreateNotFoundResponse("Unauthorized - Token does not active");
            }
            return ApiResponse<UserDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Validate token successfully", user);
        }

        public async Task<ApiResponse> KeepAlive(JwtOptions jwtOptions)
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.
                Headers.
                Authorization.
                FirstOrDefault()?.Split(" ").
                Last();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return ApiResponse.CreateUnauthorizedResponse("Unauthorized - Empty token");
            }

            ClaimsPrincipal? principal;
            try
            {
                principal = JwtExtensions.ValidateToken(accessToken, jwtOptions);
            }
            catch (SecurityTokenException)
            {
                return ApiResponse.CreateUnauthorizedResponse("Unauthorized - Invalid ClaimsPrincipal");
            }

            var accountIdString = principal.FindFirst(AppClaimTypes.Id)?.Value;
            if (!Guid.TryParse(accountIdString, out var accountId) || accountId == Guid.Empty)
            {
                return ApiResponse.CreateUnauthorizedResponse("Unauthorized - Invalid Account ID");
            }
            var user = await unitOfWork.UserAuth.GetByIdAsync(accountId);
            if (user == null)
            {
                return ApiResponse.CreateNotFoundResponse("Unauthorized - Account ID does not exist");
            }
            var tokens = await unitOfWork.RefreshTokens.GetActiveTokensByUserIdAsync(accountId);
            if (tokens.Count == 0)
            {
                return ApiResponse.CreateNotFoundResponse("Does not found any ative token, please login again!");
            }
            foreach (var token in tokens)
            {
                var newToken = JwtExtensions.GenerateAccessToken(user, jwtOptions);
                var result = await unitOfWork.RefreshTokens.RotateTokenAsync(token.Id, newToken, string.Empty);
                if (result == null)
                {
                    return ApiResponse.CreateNotFoundResponse($"Not found token with id {token.Id}");
                }
            }
            return ApiResponse.CreateSuccessResponse("Rotate token successfully!");
        }
    }
}
