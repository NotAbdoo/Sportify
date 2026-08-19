using Sportify.Models;
using Microsoft.EntityFrameworkCore;

namespace Sportify
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<GymAd> GymAds { get; set; }
        public DbSet<GymClass> GymClasses { get; set; }
        public DbSet<GymOffer> GymOffers { get; set; }

        //For the config files
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<WishlistItem>(builder =>
            {
                builder.HasKey(w => new { w.UserID, w.ProductID });

                builder.HasOne(w => w.User)
                    .WithMany()
                    .HasForeignKey(w => w.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(w => w.Product)
                    .WithMany()
                    .HasForeignKey(w => w.ProductID)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.Property(w => w.CreatedAt)
                    .IsRequired();
            });
        }

    }
}
