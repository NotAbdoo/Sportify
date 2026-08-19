using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderReviewConfig : IEntityTypeConfiguration<OrderReview>
{
    public void Configure(EntityTypeBuilder<OrderReview> builder)
    {
        builder.HasKey(r => r.OrderReviewId);

        builder.Property(r => r.Rating).IsRequired().HasColumnType("DECIMAL(2,1)");
        builder.Property(r => r.Comment).HasColumnType("TEXT");
        builder.Property(r => r.TheWantedOrder).IsRequired();
        builder.Property(r => r.OnTime).IsRequired();
        builder.Property(r => r.GoodStatus).IsRequired();

        builder.HasOne(r => r.Order)
            .WithMany(o => o.OrderReviews)
            .HasForeignKey(r => r.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new OrderReview { OrderReviewId = 1, Rating = 4.5m, TheWantedOrder = true, OnTime = true, GoodStatus = true, Comment = "Great experience, fast delivery!", OrderID = 1 },
            new OrderReview { OrderReviewId = 2, Rating = 3.0m, TheWantedOrder = true, OnTime = false, GoodStatus = true, Comment = "Items were correct but arrived late.", OrderID = 2 }
        );
    }
}