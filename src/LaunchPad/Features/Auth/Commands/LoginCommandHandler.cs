using MediatR;
using LaunchPad.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace LaunchPad.Features.Auth.Commands
{
    /// <summary>
    /// Handler for LoginCommand. Authenticates user and generates JWT token.
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IAuthService authService, IConfiguration configuration, ILogger<LoginCommandHandler> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("[COMMAND: Login] Attempting login for username: {Username}", request.Username);

                var user = await _authService.ValidateUserAsync(request.Username, request.Password);
                
                if (user == null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning("[COMMAND: Login] Invalid credentials for username: {Username} after {Duration}ms", 
                        request.Username, stopwatch.ElapsedMilliseconds);
                    
                    return new LoginResponse
                    {
                        Message = "Invalid username or password",
                        Token = string.Empty
                    };
                }

                var token = GenerateJwtToken(user.Id, user.Username);
                var expiresAt = DateTime.UtcNow.AddHours(1);

                stopwatch.Stop();
                _logger.LogInformation("[COMMAND: Login] User logged in successfully. UserID: {UserId}, Username: {Username}, Duration: {Duration}ms",
                    user.Id, user.Username, stopwatch.ElapsedMilliseconds);

                return new LoginResponse
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    Message = "Login successful"
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[COMMAND: Login] Error during login for username: {Username} after {Duration}ms",
                    request.Username, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private string GenerateJwtToken(int userId, string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "LaunchPadApi";
            var audience = jwtSettings["Audience"] ?? "LaunchPadClients";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
