using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _db;

        public CartService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CartItem>> GetCartItemsByUserIdAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            return await _db.CartItems
                .Include(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.Brand)
                .Include(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.Category)
                .Where(ci => ci.CartID == cart.CartID)
                .ToListAsync();
        }

        public async Task<CartOperationResult> AddToCartAsync(int userId, int productVariantId, int quantity)
        {
            if (quantity < 1)
                quantity = 1;

            var variant = await _db.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.ProductVariantId == productVariantId);

            if (variant == null)
            {
                return new CartOperationResult { Success = false, Message = "Product variant not found." };
            }

            if (variant.StockQuantity <= 0)
            {
                return new CartOperationResult 
                { 
                    Success = false, 
                    Message = "This product is out of stock.",
                    ProductId = variant.ProductID
                };
            }

            if (quantity > variant.StockQuantity)
                quantity = variant.StockQuantity;

            var cart = await GetOrCreateCartAsync(userId);

            var existingItem = await _db.CartItems
                .FirstOrDefaultAsync(ci =>
                    ci.CartID == cart.CartID &&
                    ci.ProductVariantId == productVariantId);

            if (existingItem == null)
            {
                _db.CartItems.Add(new CartItem
                {
                    CartID = cart.CartID,
                    ProductVariantId = productVariantId,
                    Quantity = quantity
                });
            }
            else
            {
                existingItem.Quantity += quantity;

                if (existingItem.Quantity > variant.StockQuantity)
                    existingItem.Quantity = variant.StockQuantity;
            }

            await _db.SaveChangesAsync();

            return new CartOperationResult { Success = true, Message = "Product added to cart.", ProductId = variant.ProductID };
        }

        public async Task<bool> UpdateCartItemQuantityAsync(int userId, int productVariantId, int quantity)
        {
            var cartItem = await _db.CartItems
                .Include(ci => ci.Cart)
                .Include(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(ci =>
                    ci.ProductVariantId == productVariantId &&
                    ci.Cart.UserID == userId);

            if (cartItem == null)
                return false;

            if (quantity <= 0)
            {
                _db.CartItems.Remove(cartItem);
            }
            else
            {
                if (quantity > cartItem.ProductVariant.StockQuantity)
                    quantity = cartItem.ProductVariant.StockQuantity;

                cartItem.Quantity = quantity;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productVariantId)
        {
            var cartItem = await _db.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci =>
                    ci.ProductVariantId == productVariantId &&
                    ci.Cart.UserID == userId);

            if (cartItem != null)
            {
                _db.CartItems.Remove(cartItem);
                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await _db.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart != null && cart.CartItems.Any())
            {
                _db.CartItems.RemoveRange(cart.CartItems);
                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _db.Carts
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart != null)
                return cart;

            cart = new Cart
            {
                UserID = userId,
                CartItems = new List<CartItem>()
            };

            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();

            return cart;
        }

        public async Task<bool> IsVariantInAnyCartAsync(int productVariantId)
        {
            return await _db.CartItems.AnyAsync(ci => ci.ProductVariantId == productVariantId);
        }

        public async Task<bool> AreVariantsInAnyCartAsync(List<int> productVariantIds)
        {
            if (productVariantIds == null || !productVariantIds.Any())
                return false;

            return await _db.CartItems.AnyAsync(ci => productVariantIds.Contains(ci.ProductVariantId));
        }
    }
}
