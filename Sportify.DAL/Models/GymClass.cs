using System;

namespace Sportify.Models
{
    public class GymClass
    {
        public int GymClassId { get; set; }
        public string ClassName { get; set; }        // VARCHAR(100)
        public string Description { get; set; }      // TEXT
        public string? Duration { get; set; }        // e.g. "60 min"
        public string? Price { get; set; }           // e.g. "200 EGP/month"
        public string? ImageURLs { get; set; }       // Semicolon-separated images
        public DateTime CreatedAt { get; set; }

        // FK
        public int GymAdId { get; set; }

        // Navigation
        public GymAd GymAd { get; set; }
    }
}
