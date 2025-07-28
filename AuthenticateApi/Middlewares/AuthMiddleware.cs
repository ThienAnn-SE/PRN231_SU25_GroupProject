using AppCore.BaseModel;
using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebApi.Extension;

namespace WebApi.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtOptions _jwtOptions;

        public AuthMiddleware(RequestDelegate next, IOptions<JwtOptions> jwtOptions)
        {
            _next = next;
            _jwtOptions = jwtOptions.Value;
        }


        public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
        {
            var accessToken = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                await _next(context);
                return;
            }

            ClaimsPrincipal? principal;
            try
            {
                principal = JwtExtensions.ValidateToken(accessToken, _jwtOptions);
            }
            catch (SecurityTokenException)
            {
                await _next(context);
                return;
            }

            var accountIdString = principal.FindFirst(AppClaimTypes.Id)?.Value;
            if (!Guid.TryParse(accountIdString, out var accountId) || accountId == Guid.Empty)
            {
                await _next(context);
                return;
            }

            var now = DateTime.UtcNow;

            var user = await dbContext.Users.FirstOrDefaultAsync(u => !u.DeletedAt.HasValue && u.Id == accountId);

            var token = await dbContext.RefreshTokens.FirstOrDefaultAsync(t =>
                !t.DeletedAt.HasValue &&
                t.Token.Equals(accessToken) &&
                t.UserId == accountId &&
                t.ExpiryDate > now);

            if (user == null || token == null)
            {
                await _next(context);
                return;
            }
            if (!token.IsActive)
            {
                await _next(context);
                return;
            }
            var claims = new[]
            {
            new Claim(AppClaimTypes.Id, user.Id.ToString()),
            new Claim(AppClaimTypes.Role, user.Role.ToString())
             };

            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            await _next(context);
        }

    }
}
