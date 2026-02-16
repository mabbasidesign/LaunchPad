using MediatR;
using LaunchPad.Services;

namespace LaunchPad.Features.Auth.Commands
{
    /// <summary>
    /// Handler for RegisterCommand. Registers a new user in the system.
    /// </summary>
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IUserService _userService;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(IUserService userService, ILogger<RegisterCommandHandler> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("[COMMAND: Register] Attempting registration for username: {Username}", request.Username);

                var userExists = await _userService.UserExistsAsync(request.Username);
                
                if (userExists)
                {
                    stopwatch.Stop();
                    _logger.LogWarning("[COMMAND: Register] Username already exists: {Username} after {Duration}ms",
                        request.Username, stopwatch.ElapsedMilliseconds);
                    
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                await _userService.RegisterUserAsync(request.Username, request.Password);
                
                var newUser = await _userService.GetUserByUsernameAsync(request.Username);

                stopwatch.Stop();
                _logger.LogInformation("[COMMAND: Register] User registered successfully. UserID: {UserId}, Username: {Username}, Duration: {Duration}ms",
                    newUser?.Id, request.Username, stopwatch.ElapsedMilliseconds);

                return new RegisterResponse
                {
                    Success = true,
                    Message = "User registered successfully",
                    UserId = newUser?.Id
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[COMMAND: Register] Error during registration for username: {Username} after {Duration}ms",
                    request.Username, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
