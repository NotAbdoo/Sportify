using Sportify.Models;

namespace Sportify.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> FeaturedProducts { get; set; } = new();
        public List<Brand> FeaturedBrands { get; set; } = new();
        public List<Category> SportCategories { get; set; } = new();

        public int ProductCount { get; set; }
        public int BrandCount { get; set; }
        public int UserCount { get; set; }
    }
}