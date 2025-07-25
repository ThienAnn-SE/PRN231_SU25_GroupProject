using AppCore.BaseModel;
using AppCore.Data;
using AppCore.Dtos;
using Azure;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using WebApi.Extension;

namespace WebApi.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly string _validateTokenUrl;

        public AuthMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, IOptions<AuthApiSettings> authApiOption)
        {
            _next = next;
            _httpClient = httpClientFactory.CreateClient();
            _validateTokenUrl = authApiOption.Value.ValidateTokenUrl;
        }


        public async Task Invoke(HttpContext context)
        {
            var accessToken = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                await _next(context);
                return;
            }
            var userDto = await GetUserInfoAsync(accessToken);
            if (userDto == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is invalid.");
                return;
            }
            if (userDto.Data == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(userDto.Message);
                return;
            }
            if (userDto.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(userDto.Message);
                return;
            }
            else if (userDto.Status == System.Net.HttpStatusCode.NotFound)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync(userDto.Message);
                return;
            }
            var claims = new[]
            {
                new Claim(AppClaimTypes.Id, userDto.Data.Id.ToString()),
                new Claim(AppClaimTypes.Role, userDto.Data.Role.ToString())
            };

            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            await _next(context);
        }

        private async Task<ApiResponse<UserDto>?> GetUserInfoAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _validateTokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<UserDto>?>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }

    public class AuthApiSettings
    {
        public string ValidateTokenUrl { get; set; }
    }

    public class AppClaimTypes
    {
        public const string Id = "id";
        public const string Role = "role";
    }

}
