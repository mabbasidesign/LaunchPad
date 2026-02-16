using Microsoft.AspNetCore.Mvc;
using MediatR;
using LaunchPad.DTO;
using LaunchPad.Features.Auth.Commands;

namespace LaunchPad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[LOGIN] Validation failed. Errors: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return BadRequest(ModelState);
            }

            var command = new LoginCommand(request.Username, request.Password);
            var result = await _mediator.Send(command);

            if (result.Token == string.Empty)
            {
                _logger.LogWarning("[LOGIN] Authentication failed for user {Username}. Reason: Invalid credentials", 
                    request.Username);
                return Unauthorized(new { message = result.Message });
            }

            return Ok(new { token = result.Token, expiresAt = result.ExpiresAt });
        }
    }
}
