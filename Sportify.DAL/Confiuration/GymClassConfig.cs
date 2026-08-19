using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GymClassConfig : IEntityTypeConfiguration<GymClass>
{
    public void Configure(EntityTypeBuilder<GymClass> builder)
    {
        builder.HasKey(c => c.GymClassId);

        builder.Property(c => c.ClassName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(c => c.Duration)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(c => c.Price)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(c => c.ImageURLs)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasOne(c => c.GymAd)
            .WithMany(g => g.GymClasses)
            .HasForeignKey(c => c.GymAdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
