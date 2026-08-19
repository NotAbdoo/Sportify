using Microsoft.AspNetCore.Mvc;
using Sportify.BLL.Services;
using Sportify.Models;

namespace Sportify.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email and password are required.";
                return View();
            }

            var user = await _accountService.LoginAsync(email, password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);

            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string password,
            string role = "Customer")
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please fill all required fields.";
                return View();
            }

            // Only allow Customer or GymOwner — never Admin from registration
            if (role != "Customer" && role != "GymOwner")
                role = "Customer";

            bool emailExists = await _accountService.IsEmailRegisteredAsync(email);

            if (emailExists)
            {
                ViewBag.Error = "This email is already registered.";
                return View();
            }

            var user = new User
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = email.Trim().ToLower(),
                Phone = phone ?? "",
                Address = address ?? "",
                Role = role
            };

            var registeredUser = await _accountService.RegisterAsync(user, password);

            HttpContext.Session.SetInt32("UserID", registeredUser.UserID);
            HttpContext.Session.SetString("UserName", $"{registeredUser.FirstName} {registeredUser.LastName}");
            HttpContext.Session.SetString("UserEmail", registeredUser.Email);
            HttpContext.Session.SetString("UserRole", registeredUser.Role);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _accountService.GetProfileAsync(userId.Value);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}