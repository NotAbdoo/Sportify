using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;

        public CategoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _db.Categories
                .Include(c => c.ParentCategory)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesByParentNameAsync(string parentName)
        {
            return await _db.Categories
                .Include(c => c.Products)
                .Where(c => c.ParentCategory != null && c.ParentCategory.Name == parentName)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _db.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        }

        public async Task<Category?> GetCategoryWithDetailsAsync(int categoryId)
        {
            return await _db.Categories
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await GetCategoryByIdAsync(categoryId);
            if (category != null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
            }
        }
    }
}
