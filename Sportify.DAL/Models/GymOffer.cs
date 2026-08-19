using System;

namespace Sportify.Models
{
    public class GymOffer
    {
        public int GymOfferId { get; set; }
        public string Title { get; set; }            // VARCHAR(150)
        public string Description { get; set; }      // TEXT
        public string? DiscountText { get; set; }    // e.g. "30% OFF", "Free Trial"
        public DateTime? ValidUntil { get; set; }    // Expiry date (optional)
        public DateTime CreatedAt { get; set; }

        // FK
        public int GymAdId { get; set; }

        // Navigation
        public GymAd GymAd { get; set; }
    }
}
