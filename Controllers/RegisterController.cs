using Microsoft.AspNetCore.Mvc;
using LaunchPad.Services;
using LaunchPad.DTO;

namespace LaunchPad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(IUserService userService, ILogger<RegisterController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration request");
                return BadRequest(ModelState);
            }

            if (await _userService.UserExistsAsync(request.Username))
            {
                _logger.LogWarning("Registration failed: User {Username} already exists", request.Username);
                return BadRequest("Username already exists.");
            }

            await _userService.RegisterUserAsync(request.Username, request.Password);
            _logger.LogInformation("User {Username} registered successfully", request.Username);
            return Ok("User registered successfully.");
        }
    }
}
