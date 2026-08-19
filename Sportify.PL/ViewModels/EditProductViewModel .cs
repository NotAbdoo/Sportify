using Sportify.Models;

namespace Sportify.ViewModels
{
    public class EditProductViewModel
    {
        public Product Product { get; set; } = new Product();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}