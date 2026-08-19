using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Models
{
    public class OrderItem
    {
        // Composite PKs
        public int OrderID { get; set; }
        public int ProductVariantID { get; set; }

        public int Quantity { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
