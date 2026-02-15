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
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("[REGISTER] Request started. Username: {Username}", request.Username);

            if (!ModelState.IsValid)
            {
                stopwatch.Stop();
                _logger.LogWarning("[REGISTER] Validation failed for username {Username}. Duration: {Duration}ms. Errors: {Errors}", 
                    request.Username, 
                    stopwatch.ElapsedMilliseconds,
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return BadRequest(ModelState);
            }

            if (await _userService.UserExistsAsync(request.Username))
            {
                stopwatch.Stop();
                _logger.LogWarning("[REGISTER] Username already exists. Username: {Username}. Duration: {Duration}ms", 
                    request.Username, stopwatch.ElapsedMilliseconds);
                return BadRequest("Username already exists.");
            }

            try
            {
                await _userService.RegisterUserAsync(request.Username, request.Password);
                stopwatch.Stop();
                _logger.LogInformation("[REGISTER] User registered successfully. Username: {Username}. Duration: {Duration}ms", 
                    request.Username, stopwatch.ElapsedMilliseconds);
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[REGISTER] Registration failed for username {Username}. Duration: {Duration}ms. Exception: {ExceptionMessage}", 
                    request.Username, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }
    }
}
