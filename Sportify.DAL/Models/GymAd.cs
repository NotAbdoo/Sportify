using System;
using System.Collections.Generic;

namespace Sportify.Models
{
    public class GymAd
    {
        public int GymAdId { get; set; }

        public string GymName { get; set; }         // VARCHAR(100)

        public string Description { get; set; }      // TEXT — short overview

        public string? AboutUs { get; set; }         // TEXT — detailed about section

        public string Location { get; set; }         // VARCHAR(255)

        public string ContactNumber { get; set; }    // VARCHAR(15)

        public string? Website { get; set; }         // Optional website URL

        public string? WorkingHours { get; set; }    // e.g. "6 AM – 11 PM"

        public string? ImageURL { get; set; }        // Main gym image (semicolon-separated for gallery)

        public DateTime CreatedAt { get; set; }

        public bool IsApproved { get; set; } = false;  // Requires Admin approval

        public bool IsAdminCreated { get; set; } = false; // True if admin added this gym directly

        // FK
        public int UserID { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public ICollection<GymClass> GymClasses { get; set; }
        public ICollection<GymOffer> GymOffers { get; set; }
    }
}
