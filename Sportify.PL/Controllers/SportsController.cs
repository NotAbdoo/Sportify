using Microsoft.AspNetCore.Mvc;
using Sportify.BLL.Services;
using Sportify.Models;
using System.Threading.Tasks;

namespace Sportify.Controllers
{
    public class SportsController : Controller
    {
        private readonly ICategoryService _categoryService;

        public SportsController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var sports = await _categoryService.GetCategoriesByParentNameAsync("Sports");
            return View(sports);
        }
    }
}