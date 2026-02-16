using MediatR;
using LaunchPad.Models;

namespace LaunchPad.Features.Auth.Commands
{
    /// <summary>
    /// Command to authenticate a user and return JWT token information.
    /// </summary>
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public LoginCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    /// <summary>
    /// Response model containing JWT token details.
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
