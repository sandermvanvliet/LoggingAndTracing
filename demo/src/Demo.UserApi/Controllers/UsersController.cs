using Microsoft.AspNetCore.Mvc;

namespace Demo.UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("by-vin/{vin}")]
        public IActionResult GetUserByVin()
        {
            return Ok(new User {Name = "Joe Blogs"});
        }
    }
}