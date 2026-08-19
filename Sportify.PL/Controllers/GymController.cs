using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sportify.BLL.Services;
using Sportify.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sportify.Controllers
{
    public class GymController : Controller
    {
        private readonly IGymAdService _gymAdService;
        private readonly IWebHostEnvironment _environment;

        public GymController(IGymAdService gymAdService, IWebHostEnvironment environment)
        {
            _gymAdService = gymAdService;
            _environment = environment;
        }

        private int? CurrentUserId() => HttpContext.Session.GetInt32("UserID");
        private string? CurrentRole() => HttpContext.Session.GetString("UserRole");
        private bool IsGymOwner() => CurrentRole() == "GymOwner";
        private bool IsAdmin() => CurrentRole() == "Admin";
        private bool IsLoggedIn() => CurrentUserId() != null;

        // ══════════════════════════════════════════════════════════════════════
        // PUBLIC PAGES
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>All approved gym listings (public)</summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ads = await _gymAdService.GetAllApprovedAdsAsync();
            return View(ads);
        }

        /// <summary>Detailed gym profile page (public)</summary>
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var gym = await _gymAdService.GetAdWithDetailsAsync(id);
            if (gym == null || !gym.IsApproved)
                return NotFound();
            return View(gym);
        }

        // ══════════════════════════════════════════════════════════════════════
        // GYMOWNER — My Ads Dashboard
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> MyAds()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) { TempData["Error"] = "Only Gym Owners can access this page."; return RedirectToAction("Index", "Home"); }

            var ads = await _gymAdService.GetAdsByUserIdAsync(CurrentUserId()!.Value);
            return View(ads);
        }

        // ── Create Gym Ad ─────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");
            return View(new GymAd());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GymAd ad, IFormFile? gymImage)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            ad.UserID = CurrentUserId()!.Value;
            if (gymImage != null && gymImage.Length > 0)
            {
                var saved = await SaveImage(gymImage, "gyms");
                if (saved != null) ad.ImageURL = saved;
            }

            await _gymAdService.CreateAdAsync(ad);
            TempData["Success"] = "Your gym ad has been submitted and is awaiting admin approval.";
            return RedirectToAction("MyAds");
        }

        // ── Edit Gym Ad ───────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var ad = await _gymAdService.GetAdByIdAsync(id);
            if (ad == null || ad.UserID != CurrentUserId()!.Value) return NotFound();
            return View(ad);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GymAd ad, IFormFile? gymImage)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var existing = await _gymAdService.GetAdByIdAsync(id);
            if (existing == null || existing.UserID != CurrentUserId()!.Value) return NotFound();

            existing.GymName = ad.GymName;
            existing.Description = ad.Description;
            existing.AboutUs = ad.AboutUs;
            existing.Location = ad.Location;
            existing.ContactNumber = ad.ContactNumber;
            existing.Website = ad.Website;
            existing.WorkingHours = ad.WorkingHours;

            if (gymImage != null && gymImage.Length > 0)
            {
                var saved = await SaveImage(gymImage, "gyms");
                if (saved != null) existing.ImageURL = saved;
            }

            await _gymAdService.UpdateAdAsync(existing);
            TempData["Success"] = "Your gym ad has been updated and is awaiting re-approval.";
            return RedirectToAction("MyAds");
        }

        // ── Delete Gym Ad ─────────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var ad = await _gymAdService.GetAdByIdAsync(id);
            if (ad == null || ad.UserID != CurrentUserId()!.Value) return NotFound();

            await _gymAdService.DeleteAdAsync(id);
            TempData["Success"] = "Gym ad deleted successfully.";
            return RedirectToAction("MyAds");
        }

        // ══════════════════════════════════════════════════════════════════════
        // GYMOWNER — Manage Profile (classes + offers) for an approved gym
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> ManageProfile(int gymId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var gym = await _gymAdService.GetAdWithDetailsAsync(gymId);
            if (gym == null || gym.UserID != CurrentUserId()!.Value) return NotFound();
            if (!gym.IsApproved) { TempData["Error"] = "Your gym must be approved before you can manage its profile."; return RedirectToAction("MyAds"); }

            return View(gym);
        }

        // ── Classes ───────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> AddClass(int gymId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var gym = await _gymAdService.GetAdByIdAsync(gymId);
            if (gym == null || gym.UserID != CurrentUserId()!.Value || !gym.IsApproved) return NotFound();

            ViewBag.GymId = gymId;
            ViewBag.GymName = gym.GymName;
            return View(new GymClass { GymAdId = gymId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClass(GymClass gymClass, List<IFormFile>? classImages)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var gym = await _gymAdService.GetAdByIdAsync(gymClass.GymAdId);
            if (gym == null || gym.UserID != CurrentUserId()!.Value) return NotFound();

            var savedUrls = new List<string>();
            if (classImages != null)
            {
                foreach (var img in classImages)
                {
                    if (img.Length > 0)
                    {
                        var saved = await SaveImage(img, "classes");
                        if (saved != null) savedUrls.Add(saved);
                    }
                }
            }

            if (savedUrls.Any())
                gymClass.ImageURLs = string.Join(";", savedUrls);

            await _gymAdService.CreateClassAsync(gymClass);
            TempData["Success"] = "Class added successfully!";
            return RedirectToAction("ManageProfile", new { gymId = gymClass.GymAdId });
        }

        [HttpGet]
        public async Task<IActionResult> EditClass(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var gymClass = await _gymAdService.GetClassByIdAsync(id);
            if (gymClass == null) return NotFound();

            var gym = await _gymAdService.GetAdByIdAsync(gymClass.GymAdId);
            if (gym == null || gym.UserID != CurrentUserId()!.Value) return NotFound();

            ViewBag.GymId = gymClass.GymAdId;
            ViewBag.GymName = gym.GymName;
            return View(gymClass);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClass(int id, GymClass gymClass, List<IFormFile>? classImages)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var existing = await _gymAdService.GetClassByIdAsync(id);
            if (existing == null) return NotFound();

            var gym = await _gymAdService.GetAdByIdAsync(existing.GymAdId);
            if (gym == null || gym.UserID != CurrentUserId()!.Value) return NotFound();

            existing.ClassName = gymClass.ClassName;
            existing.Description = gymClass.Description;
            existing.Duration = gymClass.Duration;
            existing.Price = gymClass.Price;

            if (classImages != null && classImages.Any(f => f.Length > 0))
            {
                var savedUrls = new List<string>();
                foreach (var img in classImages)
                {
                    if (img.Length > 0)
                    {
                        var saved = await SaveImage(img, "classes");
                        if (saved != null) savedUrls.Add(saved);
                    }
                }
                if (savedUrls.Any())
                    existing.ImageURLs = string.Join(";", savedUrls);
            }

            await _gymAdService.UpdateClassAsync(existing);
            TempData["Success"] = "Class updated!";
            return RedirectToAction("ManageProfile", new { gymId = existing.GymAdId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClass(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var gymClass = await _gymAdService.GetClassByIdAsync(id);
            if (gymClass == null) return NotFound();

            var gym = await _gymAdService.GetAdByIdAsync(gymClass.GymAdId);
            if (gym == null || gym.UserID != CurrentUserId()!.Value) return NotFound();

            int gymId = gymClass.GymAdId;
            await _gymAdService.DeleteClassAsync(id);
            TempData["Success"] = "Class deleted.";
            return RedirectToAction("ManageProfile", new { gymId });
        }

        // ── Offers ────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> AddOffer(int gymId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var gym = await _gymAdService.GetAdByIdAsync(gymId);
            if (gym == null || gym.UserID != CurrentUserId()!.Value || !gym.IsApproved) return NotFound();

            ViewBag.GymId = gymId;
            ViewBag.GymName = gym.GymName;
            return View(new GymOffer { GymAdId = gymId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOffer(GymOffer offer)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var gym = await _gymAdService.GetAdByIdAsync(offer.GymAdId);
            if (gym == null || gym.UserID != CurrentUserId()!.Value) return NotFound();

            await _gymAdService.CreateOfferAsync(offer);
            TempData["Success"] = "Offer added!";
            return RedirectToAction("ManageProfile", new { gymId = offer.GymAdId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOffer(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsGymOwner()) return RedirectToAction("Index");

            var offer = await _gymAdService.GetOfferByIdAsync(id);
            if (offer == null) return NotFound();

            var gym = await _gymAdService.GetAdByIdAsync(offer.GymAdId);
            if (gym == null || gym.UserID != CurrentUserId()!.Value) return NotFound();

            int gymId = offer.GymAdId;
            await _gymAdService.DeleteOfferAsync(id);
            TempData["Success"] = "Offer deleted.";
            return RedirectToAction("ManageProfile", new { gymId });
        }

        // ══════════════════════════════════════════════════════════════════════
        // SHARED HELPER
        // ══════════════════════════════════════════════════════════════════════

        private async Task<string?> SaveImage(IFormFile image, string folderName)
        {
            const long maxBytes = 5 * 1024 * 1024;
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext) || image.Length > maxBytes) return null;

            var folder = Path.Combine(_environment.WebRootPath, "Images", folderName);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await image.CopyToAsync(stream);

            return $"/Images/{folderName}/{fileName}";
        }
    }
}
