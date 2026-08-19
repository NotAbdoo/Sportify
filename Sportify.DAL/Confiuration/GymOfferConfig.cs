using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GymOfferConfig : IEntityTypeConfiguration<GymOffer>
{
    public void Configure(EntityTypeBuilder<GymOffer> builder)
    {
        builder.HasKey(o => o.GymOfferId);

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(o => o.Description)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(o => o.DiscountText)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(o => o.ValidUntil)
            .IsRequired(false);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.HasOne(o => o.GymAd)
            .WithMany(g => g.GymOffers)
            .HasForeignKey(o => o.GymAdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
