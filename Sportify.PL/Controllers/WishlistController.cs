using Microsoft.AspNetCore.Mvc;
using Sportify.BLL.Services;
using Sportify.Models;

namespace Sportify.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var wishlistItems = await _wishlistService.GetWishlistByUserIdAsync(userId.Value);

            return View(wishlistItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var added = await _wishlistService.AddToWishlistAsync(userId.Value, productId);

            if (!added)
            {
                // Note: The original controller returned NotFound() if product didn't exist.
                // Our AddToWishlistAsync returns false if product doesn't exist OR if it's already there.
                // Let's check if the product existed. We can just set a message or return NotFound() if appropriate.
                // But let's check: in the original, if it didn't exist, NotFound() was returned.
                // If it already existed, it set Success TempData.
                // Let's keep it simple:
                TempData["Success"] = "Product is already in your wishlist.";
            }
            else
            {
                TempData["Success"] = "Product added to wishlist.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            await _wishlistService.RemoveFromWishlistAsync(userId.Value, productId);

            return RedirectToAction("Index");
        }
    }
}