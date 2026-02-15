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
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login request");
                return BadRequest(ModelState);
            }

            var user = await _authService.ValidateUserAsync(request.Username, request.Password);
            if (user == null)
            {
                _logger.LogWarning("Login failed for user: {Username}", request.Username);
                return Unauthorized("Invalid username or password.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username)
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

            _logger.LogInformation("Login successful for user: {Username}", request.Username);
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
