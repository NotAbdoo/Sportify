using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sportify.BLL.Services;
using Sportify.Models;
using System.Threading.Tasks;

namespace Sportify.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductDetailsAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Men()
        {
            var products = await _productService.GetProductsByRootCategoryAsync("Men");
            ViewBag.CategoryName = "Men";
            ViewBag.CategoryDescription = "Explore our exclusive Men's collection.";
            return View(products);
        }

        public async Task<IActionResult> Women()
        {
            var products = await _productService.GetProductsByRootCategoryAsync("Women's", "Women");
            ViewBag.CategoryName = "Women";
            ViewBag.CategoryDescription = "Explore our exclusive Women's collection.";
            return View(products);
        }

        public async Task<IActionResult> Kids()
        {
            var products = await _productService.GetProductsByRootCategoryAsync("Kids");
            ViewBag.CategoryName = "Kids";
            ViewBag.CategoryDescription = "Explore our exclusive Kids' collection.";
            return View(products);
        }

        public async Task<IActionResult> ByBrand(int id)
        {
            var products = await _productService.GetProductsByBrandAsync(id);
            return View("Index", products);
        }

        public async Task<IActionResult> ByCategory(int id)
        {
            var products = await _productService.GetProductsByCategoryAsync(id);
            return View("Index", products);
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Index");

            var products = await _productService.SearchProductsAsync(query);

            ViewBag.SearchQuery = query.Trim();

            return View("Index", products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, decimal rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] = "Please select a valid rating and write a comment.";
                return RedirectToAction("Details", new { id = productId });
            }

            var review = new ProductReview
            {
                ProductID = productId,
                UserID = userId.Value,
                Rating = rating,
                Comment = comment.Trim()
            };

            await _productService.AddOrUpdateReviewAsync(review);

            TempData["Success"] = "Your review has been submitted successfully.";
            return RedirectToAction("Details", new { id = productId });
        }
    }
}