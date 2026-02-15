using LaunchPad.Models;
using System.Threading.Tasks;

namespace LaunchPad.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string username, string password);
    }
}