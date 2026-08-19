namespace Sportify.Models
{
    public class WishlistItem
    {
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}