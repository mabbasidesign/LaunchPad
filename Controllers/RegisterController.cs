using Microsoft.AspNetCore.Mvc;
using MediatR;
using LaunchPad.DTO;
using LaunchPad.Features.Auth.Commands;

namespace LaunchPad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(IMediator mediator, ILogger<RegisterController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[REGISTER] Validation failed for username {Username}. Errors: {Errors}", 
                    request.Username, 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return BadRequest(ModelState);
            }

            var command = new RegisterCommand(request.Username, request.Password);
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                _logger.LogWarning("[REGISTER] Registration failed for username {Username}. Reason: {Message}", 
                    request.Username, result.Message);
                return BadRequest(new { message = result.Message });
            }

            _logger.LogInformation("[REGISTER] User registered successfully. Username: {Username}. UserId: {UserId}", 
                request.Username, result.UserId);
            return Ok(new { message = result.Message, userId = result.UserId });
        }
    }
}
