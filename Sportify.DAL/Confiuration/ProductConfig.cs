using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.ProductID);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.ImageURL)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.ProductVariants)
            .WithOne(pv => pv.Product)
            .HasForeignKey(pv => pv.ProductID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductReviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Product { ProductID = 1, Name = "Nike Air Max", Description = "Iconic Air Max sneakers.", ImageURL = "https://cdn.example.com/airmax.png", CreatedAt = new DateTime(2024, 1, 1), CategoryID = 6, BrandID = 1, CreatedBy = "Admin" },
            new Product { ProductID = 2, Name = "Adidas Ultraboost", Description = "High performance running shoe.", ImageURL = "https://cdn.example.com/ultraboost.png", CreatedAt = new DateTime(2024, 1, 5), CategoryID = 6, BrandID = 2, CreatedBy = "Admin" },
            new Product { ProductID = 3, Name = "Puma Slim Fit Shirt", Description = "Casual slim-fit cotton shirt.", ImageURL = "https://cdn.example.com/shirt.png", CreatedAt = new DateTime(2024, 1, 10), CategoryID = 5, BrandID = 3, CreatedBy = "Admin" },
            new Product { ProductID = 4, Name = "Sony Headphones", Description = "Latest Sony flagship speakers.", ImageURL = "https://cdn.example.com/s24.png", CreatedAt = new DateTime(2024, 2, 1), CategoryID = 9, BrandID = 5, CreatedBy = "Admin" },
            new Product { ProductID = 5, Name = "Jordan Shorts", Description = "Iconic Jordan Shorts.", ImageURL = "https://cdn.example.com/iphone15.png", CreatedAt = new DateTime(2024, 2, 10), CategoryID = 5, BrandID = 4, CreatedBy = "Admin" },
            new Product { ProductID = 6, Name = "Adidas ball", Description = "Adidas football", ImageURL = "https://cdn.example.com/iphone15.png", CreatedAt = new DateTime(2024, 2, 10), CategoryID = 10, BrandID = 2, CreatedBy = "Admin" }
        );
    }   
}