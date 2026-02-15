using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LaunchPad.Services;
using LaunchPad.DTO;

namespace LaunchPad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("[LOGIN] Request started. Username: {Username}", request.Username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[LOGIN] Validation failed. Duration: {Duration}ms. Errors: {Errors}", 
                    stopwatch.ElapsedMilliseconds, 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return BadRequest(ModelState);
            }

            var user = await _authService.ValidateUserAsync(request.Username, request.Password);
            if (user == null)
            {
                stopwatch.Stop();
                _logger.LogWarning("[LOGIN] Authentication failed for user {Username}. Duration: {Duration}ms. Reason: Invalid credentials", 
                    request.Username, stopwatch.ElapsedMilliseconds);
                return Unauthorized("Invalid username or password.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSuperSecretKey1234567890!@#$%^"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "LaunchPadAPI",
                audience: "LaunchPadUsers",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            stopwatch.Stop();
            _logger.LogInformation("[LOGIN] Authentication successful for user {Username}. Duration: {Duration}ms. UserId: {UserId}", 
                request.Username, stopwatch.ElapsedMilliseconds, user.Id);
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
