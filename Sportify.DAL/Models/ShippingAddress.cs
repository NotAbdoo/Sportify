using Sportify.Confiuration.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportify.Models
{
    public class ShippingAddress
    {
        [Key]
        public int ShippingAddressId { get; set; } // Identity 

        [Required, MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(20)]
        public string? AlternativePhone { get; set; }

        public string? Notes { get; set; }

        [Required, MaxLength(50)]
        public string City { get; set; }

        [Column(TypeName = "tinyint")]
        public byte Apartment { get; set; }

        [Column(TypeName = "tinyint")]
        public byte Floor { get; set; }

        [Required, MaxLength(100)]
        public string Area { get; set; }

        [Required, MaxLength(150)]
        public string Street { get; set; }

        [Column(TypeName = "tinyint")]
        public byte Building { get; set; }

        [Required]
        public EgyptGovernorate Governorate { get; set; }

        [Required]
        public Countries Country { get; set; }

        // FK
        [ForeignKey("User")]
        public int UserID { get; set; }

        // Navigation
        public User? User { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}