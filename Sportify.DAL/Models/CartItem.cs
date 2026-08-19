using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Models
{
    public class CartItem
    {
        // Composite PK
        public int CartID { get; set; }
        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        // Navigation Properties
        public Cart Cart { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
