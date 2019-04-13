using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Demo.UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger _logger;

        public UsersController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet("by-vin/{vin}")]
        public IActionResult GetUserByVin(string vin)
        {
            _logger.Information($"Trying to find user for {vin}");
            
            return Ok(new User {Name = "Joe Blogs"});
        }
    }
}