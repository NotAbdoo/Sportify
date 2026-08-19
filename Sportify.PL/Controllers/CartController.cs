using Microsoft.AspNetCore.Mvc;
using Sportify.BLL.Services;
using Sportify.Models;

namespace Sportify.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int? CurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserID");
        }

        public async Task<IActionResult> Index()
        {
            var userId = CurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartService.GetCartItemsByUserIdAsync(userId.Value);

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productVariantId, int quantity = 1)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var result = await _cartService.AddToCartAsync(userId.Value, productVariantId, quantity);

            if (!result.Success)
            {
                if (result.Message == "This product is out of stock.")
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Details", "Products", new { id = result.ProductId });
                }
                return NotFound();
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int productVariantId, int quantity)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var success = await _cartService.UpdateCartItemQuantityAsync(userId.Value, productVariantId, quantity);

            if (!success)
                return NotFound();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productVariantId)
        {
            var userId = CurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            await _cartService.RemoveFromCartAsync(userId.Value, productVariantId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userId = CurrentUserId();

            if (userId == null)
                return RedirectToAction("Login", "Account");

            await _cartService.ClearCartAsync(userId.Value);

            return RedirectToAction("Index");
        }
    }
}