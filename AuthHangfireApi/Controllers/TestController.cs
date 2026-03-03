using AuthHangfireApi.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthHangfireApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public() => Ok("This is a public endpoint");

        [Authorize] // Any logged-in user
        [HttpGet("secure")]
        public IActionResult Secure() => Ok($"Hello {User.Identity!.Name}, you are authenticated");

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("admin")]
        public IActionResult AdminOnly() => Ok($"Hello {User.Identity!.Name}, you are an Admin");

        [Authorize(Roles = Roles.User)]
        [HttpGet("user")]
        public IActionResult UserOnly() => Ok($"Hello {User.Identity!.Name}, you are a User");

        [Authorize(Policy = "UserOrAdmin")]
        [HttpGet("policy")]
        public IActionResult PolicyTest() => Ok($"Hello {User.Identity!.Name}, you passed the policy");

        //[Authorize(Policy = "UserOrAdmin")]
        //[HttpGet("policy")]
        //public IActionResult PolicyTest() => Ok($"Hello {User.Identity!.Name}, you passed the policy");
    }
}
