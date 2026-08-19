using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _db;

        public WishlistService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<WishlistItem>> GetWishlistByUserIdAsync(int userId)
        {
            return await _db.WishlistItems
                .Include(w => w.Product)
                    .ThenInclude(p => p.Brand)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Category)
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductVariants)
                .Where(w => w.UserID == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> AddToWishlistAsync(int userId, int productId)
        {
            var productExists = await _db.Products.AnyAsync(p => p.ProductID == productId);
            if (!productExists)
                return false;

            var alreadyExists = await _db.WishlistItems
                .AnyAsync(w => w.UserID == userId && w.ProductID == productId);

            if (!alreadyExists)
            {
                _db.WishlistItems.Add(new WishlistItem
                {
                    UserID = userId,
                    ProductID = productId,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                return true; // Added
            }

            return false; // Already existed
        }

        public async Task<bool> RemoveFromWishlistAsync(int userId, int productId)
        {
            var wishlistItem = await _db.WishlistItems
                .FirstOrDefaultAsync(w => w.UserID == userId && w.ProductID == productId);

            if (wishlistItem != null)
            {
                _db.WishlistItems.Remove(wishlistItem);
                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
