using Sportify.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sportify.BLL.Services
{
    public interface IGymAdService
    {
        // ── Public / GymOwner ──────────────────────────────────────────────────
        Task<List<GymAd>> GetAllApprovedAdsAsync();
        Task<List<GymAd>> GetAdsByUserIdAsync(int userId);
        Task<GymAd?> GetAdByIdAsync(int id);
        Task<GymAd?> GetAdWithDetailsAsync(int id);   // includes classes + offers
        Task CreateAdAsync(GymAd ad);
        Task UpdateAdAsync(GymAd ad);
        Task DeleteAdAsync(int id);

        // ── Admin ──────────────────────────────────────────────────────────────
        Task<List<GymAd>> GetAllAdsAsync();
        Task<bool> ApproveAdAsync(int id);
        Task<bool> RejectAdAsync(int id);
        Task<int> GetPendingAdsCountAsync();
        Task AdminCreateAdAsync(GymAd ad);            // Admin adds gym directly
        Task<List<GymAd>> GetAllApprovedAdsWithOwnerAsync(); // for admin management table

        // ── GymClass CRUD ──────────────────────────────────────────────────────
        Task<List<GymClass>> GetClassesByGymIdAsync(int gymAdId);
        Task<GymClass?> GetClassByIdAsync(int id);
        Task CreateClassAsync(GymClass gymClass);
        Task UpdateClassAsync(GymClass gymClass);
        Task DeleteClassAsync(int id);

        // ── GymOffer CRUD ──────────────────────────────────────────────────────
        Task<List<GymOffer>> GetOffersByGymIdAsync(int gymAdId);
        Task<GymOffer?> GetOfferByIdAsync(int id);
        Task CreateOfferAsync(GymOffer offer);
        Task UpdateOfferAsync(GymOffer offer);
        Task DeleteOfferAsync(int id);
    }
}
