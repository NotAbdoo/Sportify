using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GymAdConfig : IEntityTypeConfiguration<GymAd>
{
    public void Configure(EntityTypeBuilder<GymAd> builder)
    {
        builder.HasKey(g => g.GymAdId);

        builder.Property(g => g.GymName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(g => g.Location)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(g => g.ContactNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(g => g.ImageURL)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.IsApproved)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(g => g.User)
            .WithMany(u => u.GymAds)
            .HasForeignKey(g => g.UserID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
