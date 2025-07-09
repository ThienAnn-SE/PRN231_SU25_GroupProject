using AppCore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseCheckController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public DatabaseCheckController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> CheckConnection()
        {
            try
            {
                // Test if database connection works
                bool canConnect = await _dbContext.Database.CanConnectAsync();
                
                if (canConnect)
                {
                    return Ok(new { 
                        Status = "Connected", 
                        DatabaseName = _dbContext.Database.GetDbConnection().Database,
                    });
                }
                else
                {
                    return StatusCode(500, new { Status = "Cannot connect to database" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Status = "Error", 
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
    }
}