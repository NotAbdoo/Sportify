using Sportify.Confiuration.Enums;
using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sportify.Configurations
{
    public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
    {
        public void Configure(EntityTypeBuilder<ShippingAddress> entity)
        {
            // PK
            entity.HasKey(e => e.ShippingAddressId);

            entity.Property(e => e.ShippingAddressId)
                  .ValueGeneratedOnAdd(); // Identity

            // Properties
            entity.Property(e => e.Phone)
                  .IsRequired()
                  .HasMaxLength(20);
            entity.Property(e => e.AlternativePhone)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(e => e.City)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Area)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Street)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(e => e.Governorate)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasConversion<string>();

            entity.Property(e => e.Country)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasConversion<string>();

            // tinyint
            entity.Property(e => e.Apartment).HasColumnType("tinyint");
            entity.Property(e => e.Floor).HasColumnType("tinyint");
            entity.Property(e => e.Building).HasColumnType("tinyint");

            // Relationship
            entity.HasOne(e => e.User)
                  .WithMany(u => u.ShippingAddresses)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasData(
                new ShippingAddress
                {
                    ShippingAddressId = 1, 
                    Phone = "01234567891",
                    AlternativePhone = "01334567890",
                    Apartment = 10,
                    Floor = 4,
                    Area = "Sidi Gaber",
                    Street = "Corniche",
                    Building = 22,
                    City = "Al-Wasta",
                    Governorate = EgyptGovernorate.BeniSuef,
                    Country = Countries.Egypt,
                    UserID = 1 
                },
                new ShippingAddress
                {
                    ShippingAddressId = 2,
                    Phone = "01234567891",
                    AlternativePhone = "01198765432",
                    Apartment = 8,
                    Floor = 3,
                    Area = "City Center",
                    Street = "Main St",
                    Building = 7,
                    City = "Al-Fashn",
                    Governorate = EgyptGovernorate.BeniSuef,
                    Country = Countries.Egypt,
                    UserID = 2
                }
            );
        }
    }
}