using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Models
{
    public class Cart
    {
        public int CartID { get; set; }

        public int UserID { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
