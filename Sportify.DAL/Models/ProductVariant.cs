using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Models
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }
        public decimal Price { get; set; }        // DECIMAL(10,2)
        public int StockQuantity { get; set; }
        public string Color { get; set; }
        public string SKU { get; set; }
        public string Size { get; set; }          // CHAR
        public string? ImageURL { get; set; } //Multi-valued

        // FK
        public int ProductID { get; set; }

        // Navigation Properties
        public Product Product { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
