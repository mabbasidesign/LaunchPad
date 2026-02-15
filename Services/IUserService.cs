using LaunchPad.Models;
using System.Threading.Tasks;

namespace LaunchPad.Services
{
    public interface IUserService
    {
        Task<bool> UserExistsAsync(string username);
        Task<User?> GetUserByUsernameAsync(string username);
        Task RegisterUserAsync(string username, string password);
    }
}