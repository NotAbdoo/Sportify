using System.Collections.Generic;
using System.Threading.Tasks;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAllBrandsAsync();
        Task<Brand?> GetBrandByIdAsync(int brandId);
        Task CreateBrandAsync(Brand brand);
        Task UpdateBrandAsync(Brand brand);
        Task DeleteBrandAsync(int brandId);
    }
}
