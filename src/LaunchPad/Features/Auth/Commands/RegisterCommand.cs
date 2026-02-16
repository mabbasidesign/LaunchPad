using MediatR;

namespace LaunchPad.Features.Auth.Commands
{
    /// <summary>
    /// Command to register a new user.
    /// </summary>
    public class RegisterCommand : IRequest<RegisterResponse>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public RegisterCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    /// <summary>
    /// Response model for user registration.
    /// </summary>
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }
}
