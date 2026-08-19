using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductVariantConfig : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(pv => pv.ProductVariantId);

        builder.Property(pv => pv.Price)
            .IsRequired()
            .HasColumnType("DECIMAL(10,2)");

        builder.Property(pv => pv.StockQuantity)
            .IsRequired();

        builder.Property(pv => pv.ImageURL)
            .HasColumnType("nvarchar(max)");

        builder.Property(pv => pv.Color)
            .HasMaxLength(50);

        builder.Property(pv => pv.SKU)
            .HasMaxLength(100);

        builder.Property(pv => pv.Size)
            .HasMaxLength(10);

        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.ProductVariants)
            .HasForeignKey(pv => pv.ProductID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            // Nike Air Max
            new ProductVariant { ProductVariantId = 1, Price = 1800.00m, StockQuantity = 50, Color = "White", SKU = "NK-AM-WHT-42", Size = "42", ProductID = 1 },
            new ProductVariant { ProductVariantId = 2, Price = 1800.00m, StockQuantity = 30, Color = "Black", SKU = "NK-AM-BLK-43", Size = "43", ProductID = 1 },
            // Adidas Ultraboost
            new ProductVariant { ProductVariantId = 3, Price = 2200.00m, StockQuantity = 40, Color = "Grey", SKU = "AD-UB-GRY-42", Size = "42", ProductID = 2 },
            new ProductVariant { ProductVariantId = 4, Price = 2200.00m, StockQuantity = 25, Color = "Navy", SKU = "AD-UB-NVY-44", Size = "44", ProductID = 2 },
            // Puma Slim Fit Shirt
            new ProductVariant { ProductVariantId = 5, Price = 350.00m, StockQuantity = 100, Color = "White", SKU = "PM-SH-WHT-M", Size = "M", ProductID = 3 },
            new ProductVariant { ProductVariantId = 6, Price = 350.00m, StockQuantity = 80, Color = "Blue", SKU = "PM-SH-BLU-L", Size = "L", ProductID = 3 },
            // Sony Headphones
            new ProductVariant { ProductVariantId = 7, Price = 2500.00m, StockQuantity = 20, Color = "Phantom Black", SKU = "SH-H200-BLK-256", Size = "N/A", ProductID = 4 },
            new ProductVariant { ProductVariantId = 8, Price = 2700.00m, StockQuantity = 15, Color = "Cream", SKU = "SH-H200-CRM-512", Size = "N/A", ProductID = 4 },
            // Jordan Shorts
            new ProductVariant { ProductVariantId = 9, Price = 1800.00m, StockQuantity = 25, Color = "Pink", SKU = "JD-SHRT-PNK-128", Size = "L", ProductID = 5 },
            new ProductVariant { ProductVariantId = 10, Price = 1800.00m, StockQuantity = 10, Color = "Black", SKU = "JD-SHRT-BLK-256", Size = "XL", ProductID = 5 },
            // Adidas ball
            new ProductVariant { ProductVariantId = 11, Price = 2000.00m, StockQuantity = 20, Color = "Blue", SKU = "AD-BL-BLU-WC", Size = "N/A", ProductID = 6 },
            new ProductVariant { ProductVariantId = 12, Price = 2000.00m, StockQuantity = 15, Color = "Purple", SKU = "AD-BL-PRPL-WC", Size = "N/A", ProductID = 6 }
            // MacBook Pro
        );
    }
}