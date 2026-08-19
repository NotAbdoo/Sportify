using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;


public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => new { oi.OrderID, oi.ProductVariantID });

        builder.Property(oi => oi.Quantity).IsRequired();

        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oi => oi.ProductVariant)
            .WithMany(pv => pv.OrderItems)
            .HasForeignKey(oi => oi.ProductVariantID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new OrderItem { OrderID = 1, ProductVariantID = 1, Quantity = 2 },
            new OrderItem { OrderID = 2, ProductVariantID = 3, Quantity = 1 },
            new OrderItem { OrderID = 1, ProductVariantID = 5, Quantity = 1 },
            new OrderItem { OrderID = 1, ProductVariantID = 4, Quantity = 1 },
            new OrderItem { OrderID = 2, ProductVariantID = 1, Quantity = 1 },
            new OrderItem { OrderID = 2, ProductVariantID = 2, Quantity = 1 }
        );
    }
}


