using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Models
{
    public class ProductReview
    {
        public int ReviewID { get; set; }
        public decimal Rating { get; set; }       // DECIMAL(2,1)
        public DateTime CreatedAt { get; set; }
        public string Comment { get; set; }       // TEXT

        // FKs
        public int UserID { get; set; }
        public int ProductID { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Product Product { get; set; }
    }
}
