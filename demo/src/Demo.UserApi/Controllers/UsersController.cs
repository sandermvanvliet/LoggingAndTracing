using System;
using System.Net;
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
            
            if(DateTime.UtcNow.TimeOfDay.Seconds % 2 == 0)
            {
                _logger.Error("Database is down, can't get user");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            
            return Ok(new User {Name = "Joe Blogs"});
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            _logger.Information($"Trying to find user with {id}");
            
            return Ok(new User {Name = "Joe Blogs"});
        }
    }
}