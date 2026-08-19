using System.Collections.Generic;
using System.Threading.Tasks;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class CartOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ProductId { get; set; }
    }

    public interface ICartService
    {
        Task<List<CartItem>> GetCartItemsByUserIdAsync(int userId);
        Task<CartOperationResult> AddToCartAsync(int userId, int productVariantId, int quantity);
        Task<bool> UpdateCartItemQuantityAsync(int userId, int productVariantId, int quantity);
        Task<bool> RemoveFromCartAsync(int userId, int productVariantId);
        Task<bool> ClearCartAsync(int userId);
        Task<Cart> GetOrCreateCartAsync(int userId);
        Task<bool> IsVariantInAnyCartAsync(int productVariantId);
        Task<bool> AreVariantsInAnyCartAsync(List<int> productVariantIds);
    }
}
