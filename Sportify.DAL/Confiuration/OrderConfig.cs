using Sportify.Confiuration.Enums;
using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.OrderId);
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.Note).HasColumnType("TEXT");
        builder.Property(o => o.FastShiping);
        builder.Property(o => o.Status).IsRequired().HasMaxLength(50).HasConversion<string>();
        builder.Property(o => o.PaymentAmount).HasColumnType("DECIMAL(10,2)");
        builder.Property(o => o.PaymentMethod).HasMaxLength(50);
        builder.Property(o => o.PaidAt);
        builder.Property(o => o.ShipmentTrackingNumber).IsRequired();
        builder.Property(o => o.ShipmentStatus).IsRequired().HasConversion<string>();

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.ShippingAddress)
            .WithMany(sa => sa.Orders)
            .HasForeignKey(o => o.ShippingAddressID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.OrderReviews)
            .WithOne(r => r.Order)
            .HasForeignKey(o => o.OrderReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Order { OrderId = 1, CreatedAt = new DateTime(2024, 3, 1), Note = "Leave at door.", FastShiping = true, Status = OrderStatus.Delivered, PaymentAmount = 3600.00m, PaymentMethod = "Card", PaidAt = new DateTime(2026, 3, 8), ShipmentTrackingNumber = 321, ShipmentStatus= ShippingStatus.Delivered, UserID = 1, ShippingAddressID = 1},
            new Order { OrderId = 2, CreatedAt = new DateTime(2024, 3, 10), Note = "Call before delivery.", FastShiping = false, Status = OrderStatus.Pending, PaymentAmount = 2200.00m, PaymentMethod = "Cash", PaidAt = new DateTime(2026, 2, 1), ShipmentTrackingNumber = 123, ShipmentStatus = ShippingStatus.Cancelled, UserID = 2, ShippingAddressID = 2}
        );
    }
}