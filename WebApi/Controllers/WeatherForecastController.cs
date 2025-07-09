using AppCore.BaseModel;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("WeatherForecasts")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ApiResponse<RefreshTokenDto>> Get()
        {
            var refreshToken = new RefreshToken(
                "abc",
                DateTime.UtcNow.AddDays(30),
                Guid.NewGuid(),
                string.Empty
            );

            return ApiResponse<RefreshTokenDto>.CreateResponse(
                HttpStatusCode.OK,
                true,
                "Weather forecast retrieved successfully."
                
            );
        }   
    }
}
