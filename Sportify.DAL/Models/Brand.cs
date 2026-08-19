using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Models
{
    public class Brand
    {
        public int BrandID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }   // TEXT
        public string LogoURL { get; set; }       // TEXT
        public string CreatedBy{ get; set; }

        // Navigation Properties
        public ICollection<Product> Products { get; set; }
    }
}
