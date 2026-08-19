using Sportify.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Sportify.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }         // VARCHAR(255)
        public string Phone { get; set; }         // VARCHAR(11)
        public string Role { get; set; }          // VARCHAR(10)
        public string PasswordHash { get; set; }  // VARCHAR(50)
        public string FirstName { get; set; }     // VARCHAR(50)
        public string LastName { get; set; }      // VARCHAR(100)
        public DateTime CreatedAt { get; set; }
        public string Address { get; set; }       // VARCHAR(255)

        // Navigation Properties
        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<ShippingAddress> ShippingAddresses { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }
        public ICollection<UserMessage> Messages { get; set; }
        public ICollection<GymAd> GymAds { get; set; }
    }
}