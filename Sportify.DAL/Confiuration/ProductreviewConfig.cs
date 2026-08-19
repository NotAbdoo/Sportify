using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductReviewConfig : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.HasKey(pr => new { pr.UserID, pr.ProductID });

        builder.Property(r => r.Rating).IsRequired().HasColumnType("DECIMAL(2,1)");
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.Comment).HasColumnType("TEXT");

        builder.HasOne(r => r.User)
            .WithMany(u => u.ProductReviews)
            .HasForeignKey(r => r.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Product)
            .WithMany(p => p.ProductReviews)
            .HasForeignKey(r => r.ProductID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new ProductReview { ReviewID = 1, Rating = 5.0m, CreatedAt = new DateTime(2024, 3, 10), Comment = "Best sneakers I've ever bought!", UserID = 1, ProductID = 1 },
            new ProductReview { ReviewID = 2, Rating = 4.0m, CreatedAt = new DateTime(2024, 3, 20), Comment = "Very comfortable for long runs.", UserID = 2, ProductID = 2 },
            new ProductReview { ReviewID = 3, Rating = 4.5m, CreatedAt = new DateTime(2024, 3, 25), Comment = "Great phone, amazing camera quality.", UserID = 1, ProductID = 4 },
            new ProductReview { ReviewID = 4, Rating = 3.5m, CreatedAt = new DateTime(2024, 4, 5), Comment = "Nice dress but sizing runs small.", UserID = 2, ProductID = 6 },
            new ProductReview { ReviewID = 5, Rating = 5.0m, CreatedAt = new DateTime(2024, 4, 15), Comment = "Incredibly fast laptop, worth every EGP.", UserID = 2, ProductID = 5 }
        );
    }
}