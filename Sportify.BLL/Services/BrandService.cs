using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _db;

        public BrandService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            return await _db.Brands
                .Include(b => b.Products)
                .ToListAsync();
        }

        public async Task<Brand?> GetBrandByIdAsync(int brandId)
        {
            return await _db.Brands.FirstOrDefaultAsync(b => b.BrandID == brandId);
        }

        public async Task CreateBrandAsync(Brand brand)
        {
            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateBrandAsync(Brand brand)
        {
            _db.Brands.Update(brand);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteBrandAsync(int brandId)
        {
            var brand = await GetBrandByIdAsync(brandId);
            if (brand != null)
            {
                _db.Brands.Remove(brand);
                await _db.SaveChangesAsync();
            }
        }
    }
}
