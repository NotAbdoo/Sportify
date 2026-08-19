using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class GymAdService : IGymAdService
    {
        private readonly AppDbContext _db;
        public GymAdService(AppDbContext db) => _db = db;

        // ══════════════════════════════════════════════════════════════════════
        // GymAd CRUD
        // ══════════════════════════════════════════════════════════════════════

        public async Task<List<GymAd>> GetAllApprovedAdsAsync()
        {
            return await _db.GymAds
                .Include(g => g.User)
                .Include(g => g.GymClasses)
                .Include(g => g.GymOffers)
                .Where(g => g.IsApproved)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<GymAd>> GetAllAdsAsync()
        {
            return await _db.GymAds
                .Include(g => g.User)
                .Include(g => g.GymClasses)
                .Include(g => g.GymOffers)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<GymAd>> GetAllApprovedAdsWithOwnerAsync()
        {
            return await _db.GymAds
                .Include(g => g.User)
                .Include(g => g.GymClasses)
                .Include(g => g.GymOffers)
                .Where(g => g.IsApproved)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<GymAd>> GetAdsByUserIdAsync(int userId)
        {
            return await _db.GymAds
                .Include(g => g.GymClasses)
                .Include(g => g.GymOffers)
                .Where(g => g.UserID == userId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<GymAd?> GetAdByIdAsync(int id)
        {
            return await _db.GymAds
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.GymAdId == id);
        }

        public async Task<GymAd?> GetAdWithDetailsAsync(int id)
        {
            return await _db.GymAds
                .Include(g => g.User)
                .Include(g => g.GymClasses)
                .Include(g => g.GymOffers)
                .FirstOrDefaultAsync(g => g.GymAdId == id);
        }

        public async Task CreateAdAsync(GymAd ad)
        {
            ad.CreatedAt = DateTime.UtcNow;
            ad.IsApproved = false;
            _db.GymAds.Add(ad);
            await _db.SaveChangesAsync();
        }

        public async Task AdminCreateAdAsync(GymAd ad)
        {
            ad.CreatedAt = DateTime.UtcNow;
            ad.IsApproved = true;         // Admin-created gyms are auto-approved
            ad.IsAdminCreated = true;
            _db.GymAds.Add(ad);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAdAsync(GymAd ad)
        {
            // If edited by owner, reset approval; if admin edit, keep approved
            if (!ad.IsAdminCreated)
                ad.IsApproved = false;
            _db.GymAds.Update(ad);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAdAsync(int id)
        {
            var ad = await _db.GymAds.FindAsync(id);
            if (ad != null)
            {
                _db.GymAds.Remove(ad);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> ApproveAdAsync(int id)
        {
            var ad = await _db.GymAds.FindAsync(id);
            if (ad == null) return false;
            ad.IsApproved = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectAdAsync(int id)
        {
            var ad = await _db.GymAds.FindAsync(id);
            if (ad == null) return false;
            _db.GymAds.Remove(ad);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetPendingAdsCountAsync()
            => await _db.GymAds.CountAsync(g => !g.IsApproved);

        // ══════════════════════════════════════════════════════════════════════
        // GymClass CRUD
        // ══════════════════════════════════════════════════════════════════════

        public async Task<List<GymClass>> GetClassesByGymIdAsync(int gymAdId)
        {
            return await _db.GymClasses
                .Where(c => c.GymAdId == gymAdId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<GymClass?> GetClassByIdAsync(int id)
            => await _db.GymClasses.FindAsync(id);

        public async Task CreateClassAsync(GymClass gymClass)
        {
            gymClass.CreatedAt = DateTime.UtcNow;
            _db.GymClasses.Add(gymClass);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateClassAsync(GymClass gymClass)
        {
            _db.GymClasses.Update(gymClass);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteClassAsync(int id)
        {
            var c = await _db.GymClasses.FindAsync(id);
            if (c != null) { _db.GymClasses.Remove(c); await _db.SaveChangesAsync(); }
        }

        // ══════════════════════════════════════════════════════════════════════
        // GymOffer CRUD
        // ══════════════════════════════════════════════════════════════════════

        public async Task<List<GymOffer>> GetOffersByGymIdAsync(int gymAdId)
        {
            return await _db.GymOffers
                .Where(o => o.GymAdId == gymAdId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<GymOffer?> GetOfferByIdAsync(int id)
            => await _db.GymOffers.FindAsync(id);

        public async Task CreateOfferAsync(GymOffer offer)
        {
            offer.CreatedAt = DateTime.UtcNow;
            _db.GymOffers.Add(offer);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateOfferAsync(GymOffer offer)
        {
            _db.GymOffers.Update(offer);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteOfferAsync(int id)
        {
            var o = await _db.GymOffers.FindAsync(id);
            if (o != null) { _db.GymOffers.Remove(o); await _db.SaveChangesAsync(); }
        }
    }
}
