using System.Collections.Generic;
using System.Threading.Tasks;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetProductDetailsAsync(int id);
        Task<List<Product>> GetProductsByBrandAsync(int brandId);
        Task<List<Product>> GetProductsByRootCategoryAsync(params string[] names);
        Task<List<Product>> SearchProductsAsync(string query);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        
        // CRUD Product
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        // Product Variant
        Task<ProductVariant?> GetProductVariantByIdAsync(int id);
        Task CreateProductVariantAsync(ProductVariant variant);
        Task UpdateProductVariantAsync(ProductVariant variant);
        Task DeleteProductVariantAsync(int id);
        // Reviews
        Task<List<ProductReview>> GetAllReviewsAsync();
        Task<ProductReview?> GetReviewByIdAsync(int id);
        Task CreateReviewAsync(ProductReview review);
        Task UpdateReviewAsync(ProductReview review);
        Task DeleteReviewAsync(int id);
        Task AddOrUpdateReviewAsync(ProductReview review);
        // Dashboard Stats
        Task<int> GetTotalProductsCountAsync();
        Task<int> GetTotalBrandsCountAsync();
        Task<int> GetTotalCategoriesCountAsync();
        Task<int> GetTotalUsersCountAsync();
    }
}
