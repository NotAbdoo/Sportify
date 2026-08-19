using Microsoft.AspNetCore.Mvc;
using Sportify.BLL.Services;
using Sportify.Models;

namespace Sportify.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return View(brands);
        }
    }
}