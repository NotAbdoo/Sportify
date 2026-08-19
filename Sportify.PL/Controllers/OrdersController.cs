using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sportify.BLL.Services;
using Sportify.Models;
using Sportify.Confiuration.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sportify.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrdersController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        private int? CurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserID");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = CurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var orders = await _orderService.GetOrdersByUserIdAsync(userId.Value);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = CurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartService.GetCartItemsByUserIdAsync(userId.Value);
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty. Please add items before checking out.";
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.CartItems = cartItems;
            return View(new ShippingAddress { UserID = userId.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(ShippingAddress address, string? note, bool fastShipping, string paymentMethod)
        {
            var userId = CurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartService.GetCartItemsByUserIdAsync(userId.Value);
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CartItems = cartItems;
                return View(address);
            }

            try
            {
                // 1. Save shipping address
                address.UserID = userId.Value;
                var savedAddress = await _orderService.CreateShippingAddressAsync(address);

                // 2. Place Order
                var order = await _orderService.PlaceOrderAsync(
                    userId.Value,
                    savedAddress.ShippingAddressId,
                    note,
                    fastShipping,
                    paymentMethod
                );

                if (paymentMethod == "Card")
                {
                    return RedirectToAction("Payment", new { id = order.OrderId });
                }
                return RedirectToAction("Success", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while placing your order: {ex.Message}";
                ViewBag.CartItems = cartItems;
                return View(address);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            var userId = CurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserID != userId.Value)
                return NotFound();

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int id)
        {
            var userId = CurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserID != userId.Value)
                return NotFound();

            if (order.PaymentMethod != "Card")
            {
                TempData["Error"] = "This order does not require card payment.";
                return RedirectToAction("Success", new { id = order.OrderId });
            }

            if (order.PaidAt != null)
            {
                TempData["Success"] = "This order is already paid.";
                return RedirectToAction("Success", new { id = order.OrderId });
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int id, string cardName, string cardNumber, string expiryDate, string cvv)
        {
            var userId = CurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserID != userId.Value)
                return NotFound();

            bool isValid = true;
            if (string.IsNullOrWhiteSpace(cardName))
            {
                ModelState.AddModelError("CardName", "Cardholder name is required.");
                isValid = false;
            }
            
            var cleanCardNumber = (cardNumber ?? "").Replace(" ", "");
            if (cleanCardNumber.Length != 16 || !cleanCardNumber.All(char.IsDigit))
            {
                ModelState.AddModelError("CardNumber", "Card number must be exactly 16 digits.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(expiryDate) || !expiryDate.Contains("/") || expiryDate.Length != 5)
            {
                ModelState.AddModelError("ExpiryDate", "Expiry date must be in MM/YY format.");
                isValid = false;
            }
            else
            {
                var parts = expiryDate.Split('/');
                if (parts.Length != 2 || !int.TryParse(parts[0], out int month) || !int.TryParse(parts[1], out int year) || month < 1 || month > 12)
                {
                    ModelState.AddModelError("ExpiryDate", "Invalid expiry date format. Use MM/YY.");
                    isValid = false;
                }
                else
                {
                    int currentYear = DateTime.UtcNow.Year % 100;
                    int currentMonth = DateTime.UtcNow.Month;
                    if (year < currentYear || (year == currentYear && month < currentMonth))
                    {
                        ModelState.AddModelError("ExpiryDate", "The card is expired.");
                        isValid = false;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(cvv) || cvv.Length != 3 || !cvv.All(char.IsDigit))
            {
                ModelState.AddModelError("CVV", "CVV must be exactly 3 digits.");
                isValid = false;
            }

            if (!isValid)
            {
                return View("Payment", order);
            }

            await _orderService.UpdateOrderPaidStatusAsync(order.OrderId, true);

            TempData["Success"] = "Payment completed successfully! Thank you.";
            return RedirectToAction("Success", new { id = order.OrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = CurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var success = await _orderService.CancelOrderAsync(id, userId.Value);
            if (success)
            {
                TempData["Success"] = "Your order has been cancelled successfully.";
            }
            else
            {
                TempData["Error"] = "Could not cancel this order. It may have already been shipped or processed.";
            }

            return RedirectToAction("Index");
        }
    }
}
