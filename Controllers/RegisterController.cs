using Microsoft.AspNetCore.Mvc;
using LaunchPad.Services;

namespace LaunchPad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _userService.UserExistsAsync(request.Username))
            {
                return BadRequest("Username already exists.");
            }
            await _userService.RegisterUserAsync(request.Username, request.Password);
            return Ok("User registered successfully.");
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
