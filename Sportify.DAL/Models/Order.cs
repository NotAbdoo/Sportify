using Sportify.Confiuration.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Note { get; set; }          // TEXT
        public bool FastShiping { get; set; }
        public OrderStatus Status { get; set; }   //ENUM
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int ShipmentTrackingNumber { get; set; }
        public ShippingStatus ShipmentStatus { get; set; } //ENUM

        public DateTime? PaidAt { get; set; }

        // FKs
        public int UserID { get; set; }
        public int ShippingAddressID { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<OrderReview> OrderReviews { get; set; }
    }
}