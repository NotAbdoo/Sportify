using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _db;

        public AccountService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            email = email.Trim().ToLower();
            string passwordHash = HashPassword(password);

            return await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.PasswordHash == passwordHash);
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            user.PasswordHash = HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            email = email.Trim().ToLower();
            return await _db.Users.AnyAsync(u => u.Email.ToLower() == email);
        }

        public async Task<User?> GetProfileAsync(int userId)
        {
            return await _db.Users
                .Include(u => u.Orders)
                .Include(u => u.ShippingAddresses)
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task AddUserMessageAsync(int userId, string messageText)
        {
            var userMessage = new UserMessage
            {
                UserID = userId,
                MessageText = messageText,
                CreatedAt = DateTime.UtcNow
            };
            _db.UserMessages.Add(userMessage);
            await _db.SaveChangesAsync();
        }

        public async Task<List<UserMessage>> GetAllUserMessagesAsync()
        {
            return await _db.UserMessages
                .Include(um => um.User)
                .OrderByDescending(um => um.CreatedAt)
                .ToListAsync();
        }
    }
}
