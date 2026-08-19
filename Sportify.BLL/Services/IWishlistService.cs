using System.Collections.Generic;
using System.Threading.Tasks;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public interface IWishlistService
    {
        Task<List<WishlistItem>> GetWishlistByUserIdAsync(int userId);
        Task<bool> AddToWishlistAsync(int userId, int productId);
        Task<bool> RemoveFromWishlistAsync(int userId, int productId);
    }
}
