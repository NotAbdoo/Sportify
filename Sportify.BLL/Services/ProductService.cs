using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _db.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task<Product?> GetProductDetailsAsync(int id)
        {
            return await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductReviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task<List<Product>> GetProductsByBrandAsync(int brandId)
        {
            return await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.BrandID == brandId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.CategoryID == categoryId || p.Category.ParentCategoryID == categoryId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByRootCategoryAsync(params string[] names)
        {
            var lower = names.Select(n => n.ToLower()).ToArray();

            return await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                    .ThenInclude(c => c.ParentCategory)
                .Include(p => p.ProductVariants)
                .Where(p =>
                    lower.Contains(p.Category.Name.ToLower()) ||
                    (p.Category.ParentCategory != null &&
                     lower.Contains(p.Category.ParentCategory.Name.ToLower())))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Product>();

            query = query.Trim().ToLower();

            return await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p =>
                    p.Name.ToLower().Contains(query) ||
                    p.Description.ToLower().Contains(query) ||
                    p.Brand.Name.ToLower().Contains(query) ||
                    p.Category.Name.ToLower().Contains(query))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<ProductVariant?> GetProductVariantByIdAsync(int id)
        {
            return await _db.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.ProductVariantId == id);
        }

        public async Task CreateProductVariantAsync(ProductVariant variant)
        {
            _db.ProductVariants.Add(variant);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateProductVariantAsync(ProductVariant variant)
        {
            _db.ProductVariants.Update(variant);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteProductVariantAsync(int id)
        {
            var variant = await GetProductVariantByIdAsync(id);
            if (variant != null)
            {
                _db.ProductVariants.Remove(variant);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<ProductReview>> GetAllReviewsAsync()
        {
            return await _db.ProductReviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<ProductReview?> GetReviewByIdAsync(int id)
        {
            return await _db.ProductReviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReviewID == id);
        }

        public async Task CreateReviewAsync(ProductReview review)
        {
            review.CreatedAt = DateTime.UtcNow;
            _db.ProductReviews.Add(review);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(ProductReview review)
        {
            _db.ProductReviews.Update(review);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await GetReviewByIdAsync(id);
            if (review != null)
            {
                _db.ProductReviews.Remove(review);
                await _db.SaveChangesAsync();
            }
        }

        public async Task AddOrUpdateReviewAsync(ProductReview review)
        {
            var existing = await _db.ProductReviews
                .FirstOrDefaultAsync(r => r.UserID == review.UserID && r.ProductID == review.ProductID);

            if (existing != null)
            {
                existing.Rating = review.Rating;
                existing.Comment = review.Comment;
                existing.CreatedAt = DateTime.UtcNow;
                _db.ProductReviews.Update(existing);
            }
            else
            {
                review.CreatedAt = DateTime.UtcNow;
                _db.ProductReviews.Add(review);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<int> GetTotalProductsCountAsync()
        {
            return await _db.Products.CountAsync();
        }

        public async Task<int> GetTotalBrandsCountAsync()
        {
            return await _db.Brands.CountAsync();
        }

        public async Task<int> GetTotalCategoriesCountAsync()
        {
            return await _db.Categories.CountAsync();
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _db.Users.CountAsync();
        }
    }
}
