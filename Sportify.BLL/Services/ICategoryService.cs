using System.Collections.Generic;
using System.Threading.Tasks;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Category>> GetCategoriesByParentNameAsync(string parentName);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<Category?> GetCategoryWithDetailsAsync(int categoryId);
        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int categoryId);
    }
}
