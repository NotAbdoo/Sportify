using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CartConfig : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.CartID);

        builder.HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Cart { CartID = 1, UserID = 1 },
            new Cart { CartID = 2, UserID = 2 },
            new Cart { CartID = 3, UserID = 3 }
        );
    }
}