using Microsoft.AspNetCore.Mvc;
using Sportify.BLL.Services;
using Sportify.Models;
using Sportify.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Sportify.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IAccountService _accountService;

        public HomeController(
            IProductService productService,
            IBrandService brandService,
            ICategoryService categoryService,
            IAccountService accountService)
        {
            _productService = productService;
            _brandService = brandService;
            _categoryService = categoryService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = (await _productService.GetAllProductsAsync())
                .Take(4)
                .ToList();

            var featuredBrands = (await _brandService.GetAllBrandsAsync())
                .Take(5)
                .ToList();

            var sportCategories = (await _categoryService.GetCategoriesByParentNameAsync("Sports"))
                .Take(4)
                .ToList();

            var productCount = await _productService.GetTotalProductsCountAsync();
            var brandCount = await _productService.GetTotalBrandsCountAsync();
            var userCount = await _productService.GetTotalUsersCountAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedProducts = featuredProducts,
                FeaturedBrands = featuredBrands,
                SportCategories = sportCategories,
                ProductCount = productCount,
                BrandCount = brandCount,
                UserCount = userCount
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string message)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to send a message.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Message cannot be empty.";
                return View();
            }

            await _accountService.AddUserMessageAsync(userId.Value, message.Trim());
            TempData["Success"] = "Your message has been sent successfully!";
            return View();
        }
    }
}