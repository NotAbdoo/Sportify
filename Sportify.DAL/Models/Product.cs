using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }   // TEXT
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        // FKs
        public int CategoryID { get; set; }
        public int BrandID { get; set; }

        // Navigation Properties
        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }
    }
}
