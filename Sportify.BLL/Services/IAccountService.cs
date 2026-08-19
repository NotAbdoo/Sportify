using System.Threading.Tasks;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public interface IAccountService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<User> RegisterAsync(User user, string password);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<User?> GetProfileAsync(int userId);
        string HashPassword(string password);
        Task AddUserMessageAsync(int userId, string messageText);
        Task<List<UserMessage>> GetAllUserMessagesAsync();
    }
}
