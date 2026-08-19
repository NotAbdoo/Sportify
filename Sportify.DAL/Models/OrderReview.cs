using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Models
{
    public class OrderReview
    {
        public int OrderReviewId { get; set; }
        public decimal Rating { get; set; }       // DECIMAL(2,1)
        public bool TheWantedOrder { get; set; }
        public bool OnTime { get; set; }
        public bool GoodStatus { get; set; }
        public string Comment { get; set; }       // TEXT

        // FK
        public int OrderID { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
    }
}
