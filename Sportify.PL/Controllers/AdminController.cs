using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sportify.BLL.Services;
using Sportify.Models;
using Sportify.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Sportify.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IAccountService _accountService;
        private readonly IGymAdService _gymAdService;
        private readonly IWebHostEnvironment _environment;

        public AdminController(
            IProductService productService,
            IBrandService brandService,
            ICategoryService categoryService,
            IOrderService orderService,
            ICartService cartService,
            IAccountService accountService,
            IGymAdService gymAdService,
            IWebHostEnvironment environment)
        {
            _productService = productService;
            _brandService = brandService;
            _categoryService = categoryService;
            _orderService = orderService;
            _cartService = cartService;
            _accountService = accountService;
            _gymAdService = gymAdService;
            _environment = environment;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        private IActionResult NotAdmin()
        {
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin()) return NotAdmin();

            // ── Core counts ──────────────────────────────────────────────────
            ViewBag.TotalProducts   = await _productService.GetTotalProductsCountAsync();
            ViewBag.TotalBrands     = await _brandService.GetAllBrandsAsync().ContinueWith(t => t.Result.Count);
            ViewBag.TotalCategories = await _categoryService.GetAllCategoriesAsync().ContinueWith(t => t.Result.Count);
            ViewBag.TotalUsers      = await _productService.GetTotalUsersCountAsync();
            ViewBag.PendingGymAds   = await _gymAdService.GetPendingAdsCountAsync();

            var orderStats = await _orderService.GetOrderStatsAsync();
            ViewBag.TotalOrders = orderStats.TotalOrders;

            // ── Revenue ──────────────────────────────────────────────────────
            ViewBag.TotalRevenue = orderStats.TotalRevenue;
            ViewBag.ThisMonthRevenue = orderStats.ThisMonthRevenue;
            ViewBag.LastMonthRevenue = orderStats.LastMonthRevenue;

            // ── This-month order count vs last month ─────────────────────────
            ViewBag.ThisMonthOrders = orderStats.ThisMonthOrders;
            ViewBag.LastMonthOrders = orderStats.LastMonthOrders;

            // ── Monthly chart data ───────────────────────────────────────────
            ViewBag.ChartLabels  = orderStats.MonthlyChart.Select(c => c.Label).ToList();
            ViewBag.ChartOrders  = orderStats.MonthlyChart.Select(c => c.Count).ToList();
            ViewBag.ChartRevenue = orderStats.MonthlyChart.Select(c => c.Revenue).ToList();

            // ── Recent orders ─────────────────────────────────────────────────
            var recentOrders = await _orderService.GetRecentOrdersAsync(5);
            return View(recentOrders);
        }

        public async Task<IActionResult> Manage()
        {
            if (!IsAdmin()) return NotAdmin();

            ViewBag.Products = await _productService.GetAllProductsAsync();
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Brands = await _brandService.GetAllBrandsAsync();

            return View("AdminManage");
        }

        [HttpGet]
        public async Task<IActionResult> Messages()
        {
            if (!IsAdmin()) return NotAdmin();

            var messages = await _accountService.GetAllUserMessagesAsync();
            return View(messages);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            if (!IsAdmin()) return NotAdmin();

            ViewBag.Brands = await _brandService.GetAllBrandsAsync();
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(
            Product product,
            List<IFormFile> productImages,
            List<int> variantIndices,
            List<decimal> variantPrices,
            List<int> variantStockQuantities,
            List<string> variantColors,
            List<string> variantSizes,
            List<string> variantSkus)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var savedMainUrls = new List<string>();
            if (productImages != null && productImages.Count > 0)
            {
                foreach (var img in productImages)
                {
                    if (img.Length > 0)
                    {
                        var saved = await SaveImage(img, "products");
                        if (saved != null)
                            savedMainUrls.Add(saved);
                    }
                }
            }

            if (savedMainUrls.Count > 0)
            {
                product.ImageURL = string.Join(";", savedMainUrls);
            }
            else
            {
                product.ImageURL = "/Images/no-image.png";
            }

            product.CreatedBy = HttpContext.Session.GetString("UserName") ?? "Admin";

            await _productService.CreateProductAsync(product);

            if (variantColors != null)
            {
                for (int i = 0; i < variantColors.Count; i++)
                {
                    var savedVariantUrls = new List<string>();
                    
                    if (variantIndices != null && variantIndices.Count > i)
                    {
                        int rowIdx = variantIndices[i];
                        var files = Request.Form.Files.GetFiles($"variantImages_{rowIdx}");
                        foreach (var file in files)
                        {
                            if (file.Length > 0)
                            {
                                var saved = await SaveImage(file, "products");
                                if (saved != null)
                                    savedVariantUrls.Add(saved);
                            }
                        }
                    }

                    string variantImageUrls = savedVariantUrls.Count > 0 
                        ? string.Join(";", savedVariantUrls) 
                        : product.ImageURL;

                    await _productService.CreateProductVariantAsync(new ProductVariant
                    {
                        ProductID = product.ProductID,
                        Price = variantPrices[i],
                        StockQuantity = variantStockQuantities[i],
                        Color = variantColors[i],
                        Size = variantSizes[i],
                        SKU = variantSkus[i],
                        ImageURL = variantImageUrls
                    });
                }
            }

            TempData["Success"] = "Product added successfully.";
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            var viewModel = new EditProductViewModel
            {
                Product = product,
                Brands = await _brandService.GetAllBrandsAsync(),
                Categories = await _categoryService.GetAllCategoriesAsync()
            };

            return View("AdminProductEdit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(
            int id,
            Product product,
            IFormFile? productImage,
            List<int> existingVariantIds,
            List<decimal> existingVariantPrices,
            List<int> existingVariantStockQuantities,
            List<string> existingVariantColors,
            List<string> existingVariantSizes,
            List<string> existingVariantSkus,
            List<IFormFile?> existingVariantImages,
            List<decimal>? newVariantPrices,
            List<int>? newVariantStockQuantities,
            List<string>? newVariantColors,
            List<string>? newVariantSizes,
            List<string>? newVariantSkus,
            List<IFormFile?>? newVariantImages)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (id != product.ProductID)
                return BadRequest();

            var existingProduct = await _productService.GetProductByIdAsync(id);

            if (existingProduct == null)
                return NotFound();

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.BrandID = product.BrandID;
            existingProduct.CategoryID = product.CategoryID;

            if (productImage != null && productImage.Length > 0)
            {
                var saved = await SaveImage(productImage, "products");
                if (saved != null)
                    existingProduct.ImageURL = saved;
                else
                    TempData["Warning"] = "Main image was rejected (must be jpg/png/webp/gif, max 5 MB). The existing image was kept.";
            }

            for (int i = 0; i < existingVariantIds.Count; i++)
            {
                var variant = existingProduct.ProductVariants
                    .FirstOrDefault(v => v.ProductVariantId == existingVariantIds[i]);

                if (variant == null)
                    continue;

                variant.Price = existingVariantPrices[i];
                variant.StockQuantity = existingVariantStockQuantities[i];
                variant.Color = existingVariantColors[i];
                variant.Size = existingVariantSizes[i];
                variant.SKU = existingVariantSkus[i];

                if (existingVariantImages.Count > i &&
                    existingVariantImages[i] != null &&
                    existingVariantImages[i]!.Length > 0)
                {
                    variant.ImageURL = await SaveImage(existingVariantImages[i]!, "products");
                }

                await _productService.UpdateProductVariantAsync(variant);
            }

            if (newVariantColors != null)
            {
                for (int i = 0; i < newVariantColors.Count; i++)
                {
                    string? imageUrl = existingProduct.ImageURL;

                    if (newVariantImages != null &&
                        newVariantImages.Count > i &&
                        newVariantImages[i] != null &&
                        newVariantImages[i]!.Length > 0)
                    {
                        imageUrl = await SaveImage(newVariantImages[i]!, "products");
                    }

                    await _productService.CreateProductVariantAsync(new ProductVariant
                    {
                        ProductID = existingProduct.ProductID,
                        Price = newVariantPrices![i],
                        StockQuantity = newVariantStockQuantities![i],
                        Color = newVariantColors[i],
                        Size = newVariantSizes![i],
                        SKU = newVariantSkus![i],
                        ImageURL = imageUrl
                    });
                }
            }

            await _productService.UpdateProductAsync(existingProduct);

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVariant(int id, int productId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var variant = await _productService.GetProductVariantByIdAsync(id);

            if (variant == null)
                return NotFound();

            bool usedInCart = await _cartService.IsVariantInAnyCartAsync(id);
            bool usedInOrder = await _orderService.IsVariantInAnyOrderAsync(id);

            if (usedInCart || usedInOrder)
            {
                TempData["Error"] = "This variant cannot be deleted because it exists in carts or orders.";
                return RedirectToAction("EditProduct", new { id = productId });
            }

            await _productService.DeleteProductVariantAsync(id);

            TempData["Success"] = "Variant deleted successfully.";
            return RedirectToAction("EditProduct", new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!IsAdmin()) return NotAdmin();

            var product = await _productService.GetProductByIdAsync(id);

            if (product == null) return NotFound();

            var variantIds = product.ProductVariants.Select(v => v.ProductVariantId).ToList();

            bool usedInOrders = await _orderService.AreVariantsInAnyOrderAsync(variantIds);
            bool usedInCarts = await _cartService.AreVariantsInAnyCartAsync(variantIds);

            if (usedInOrders || usedInCarts)
            {
                TempData["Error"] = "This product cannot be deleted because it exists in carts or orders.";
                return RedirectToAction("Manage");
            }

            await _productService.DeleteProductAsync(id);

            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public IActionResult AddBrand()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            return View(new Brand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBrand(Brand brand, IFormFile? logoImage)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (logoImage != null && logoImage.Length > 0)
            {
                brand.LogoURL = await SaveImage(logoImage, "brands");
            }
            else
            {
                brand.LogoURL = "/Images/no-image.png";
            }

            brand.CreatedBy = HttpContext.Session.GetString("UserName") ?? "Admin";

            await _brandService.CreateBrandAsync(brand);

            TempData["Success"] = "Brand added successfully.";
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public async Task<IActionResult> EditBrand(int id)
        {
            if (!IsAdmin()) return NotAdmin();

            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null) return NotFound();

            return View("AdminBrandEdit", brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBrand(int id, Brand brand, IFormFile? logoImage)
        {
            if (!IsAdmin()) return NotAdmin();
            if (id != brand.BrandID) return BadRequest();

            var existingBrand = await _brandService.GetBrandByIdAsync(id);
            if (existingBrand == null) return NotFound();

            existingBrand.Name = brand.Name;
            existingBrand.Description = brand.Description;

            if (logoImage != null && logoImage.Length > 0)
                existingBrand.LogoURL = await SaveImage(logoImage, "brands");

            await _brandService.UpdateBrandAsync(existingBrand);

            TempData["Success"] = "Brand updated successfully.";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            if (!IsAdmin()) return NotAdmin();

            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null) return NotFound();

            var hasProducts = (await _productService.GetProductsByBrandAsync(id)).Any();
            if (hasProducts)
            {
                TempData["Error"] = "This brand cannot be deleted because it has products.";
                return RedirectToAction("Manage");
            }

            await _brandService.DeleteBrandAsync(id);

            TempData["Success"] = "Brand deleted successfully.";
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public async Task<IActionResult> AddCategory()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var categories = await _categoryService.GetAllCategoriesAsync();

            ViewBag.ParentCategories = categories;

            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(Category category)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            await _categoryService.CreateCategoryAsync(category);

            TempData["Success"] = "Category added successfully.";
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            if (!IsAdmin()) return NotAdmin();

            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            ViewBag.ParentCategories = (await _categoryService.GetAllCategoriesAsync())
                .Where(c => c.CategoryID != id)
                .ToList();

            return View("AdminCategoryEdit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            if (!IsAdmin()) return NotAdmin();
            if (id != category.CategoryID) return BadRequest();

            var existingCategory = await _categoryService.GetCategoryByIdAsync(id);
            if (existingCategory == null) return NotFound();

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.ParentCategoryID = category.ParentCategoryID;

            await _categoryService.UpdateCategoryAsync(existingCategory);

            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!IsAdmin()) return NotAdmin();

            var category = await _categoryService.GetCategoryWithDetailsAsync(id);

            if (category == null) return NotFound();

            if (category.Products.Any())
            {
                TempData["Error"] = "This category cannot be deleted because it has products.";
                return RedirectToAction("Manage");
            }

            if (category.SubCategories.Any())
            {
                TempData["Error"] = "This category cannot be deleted because it has sub-categories.";
                return RedirectToAction("Manage");
            }

            await _categoryService.DeleteCategoryAsync(id);

            TempData["Success"] = "Category deleted successfully.";
            return RedirectToAction("Manage");
        }

        private async Task<string?> SaveImage(IFormFile image, string folderName)
        {
            const long maxBytes = 5 * 1024 * 1024; // 5 MB
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!allowed.Contains(ext))
                return null;

            if (image.Length > maxBytes)
                return null;

            var targetWebRootPath = _environment.WebRootPath;
            var sourceWebRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");

            var targetFolder = Path.Combine(targetWebRootPath, "Images", folderName);
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var targetPath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(targetPath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            try
            {
                if (string.Compare(Path.GetFullPath(targetWebRootPath), Path.GetFullPath(sourceWebRootPath), StringComparison.OrdinalIgnoreCase) != 0)
                {
                    var sourceFolder = Path.Combine(sourceWebRootPath, "Images", folderName);
                    if (!Directory.Exists(sourceFolder))
                        Directory.CreateDirectory(sourceFolder);

                    var sourcePath = Path.Combine(sourceFolder, fileName);
                    System.IO.File.Copy(targetPath, sourcePath, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning copying file to source wwwroot: {ex.Message}");
            }

            return $"/Images/{folderName}/{fileName}";
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            if (!IsAdmin()) return NotAdmin();
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            if (!IsAdmin()) return NotAdmin();

            var success = await _orderService.AdminCancelOrderAsync(id);
            if (success)
            {
                TempData["Success"] = $"Order #SP{id:D4} has been cancelled successfully.";
            }
            else
            {
                TempData["Error"] = $"Could not cancel order #SP{id:D4}. It may not be in pending state.";
            }

            return Redirect(Request.Headers["Referer"].ToString() ?? "/Admin/Dashboard");
        }

        // ── Gym Ads: Review Requests (Approve / Reject) ─────────────────────

        [HttpGet]
        public async Task<IActionResult> GymAds()
        {
            if (!IsAdmin()) return NotAdmin();
            var ads = await _gymAdService.GetAllAdsAsync();
            ViewBag.PendingCount = ads.Count(a => !a.IsApproved);
            return View(ads);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveGymAd(int id)
        {
            if (!IsAdmin()) return NotAdmin();
            var success = await _gymAdService.ApproveAdAsync(id);
            TempData[success ? "Success" : "Error"] = success
                ? "✅ Gym ad approved and is now live."
                : "Gym ad not found.";
            return RedirectToAction("GymAds");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectGymAd(int id)
        {
            if (!IsAdmin()) return NotAdmin();
            var success = await _gymAdService.RejectAdAsync(id);
            TempData[success ? "Success" : "Error"] = success
                ? "Gym ad has been rejected and removed."
                : "Gym ad not found.";
            return RedirectToAction("GymAds");
        }

        // ── Gym Management: All contracted gyms + details ───────────────────

        [HttpGet]
        public async Task<IActionResult> GymManagement()
        {
            if (!IsAdmin()) return NotAdmin();
            var gyms = await _gymAdService.GetAllApprovedAdsWithOwnerAsync();
            return View(gyms);
        }

        // ── Admin: Add Gym Directly ─────────────────────────────────────────

        [HttpGet]
        public IActionResult AdminAddGym()
        {
            if (!IsAdmin()) return NotAdmin();
            return View(new Sportify.Models.GymAd());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminAddGym(Sportify.Models.GymAd ad, IFormFile? gymImage)
        {
            if (!IsAdmin()) return NotAdmin();

            // Use admin user ID
            ad.UserID = HttpContext.Session.GetInt32("UserID")!.Value;

            if (gymImage != null && gymImage.Length > 0)
            {
                var saved = await SaveImage(gymImage, "gyms");
                if (saved != null) ad.ImageURL = saved;
            }

            await _gymAdService.AdminCreateAdAsync(ad);
            TempData["Success"] = "Gym added and published successfully.";
            return RedirectToAction("GymManagement");
        }

        // ── Admin: Delete Gym ───────────────────────────────────────────────

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminDeleteGym(int id)
        {
            if (!IsAdmin()) return NotAdmin();
            await _gymAdService.DeleteAdAsync(id);
            TempData["Success"] = "Gym removed successfully.";
            return RedirectToAction("GymManagement");
        }
    }
}
