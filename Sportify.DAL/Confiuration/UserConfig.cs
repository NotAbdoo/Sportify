using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserID);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Phone)
            .HasMaxLength(11);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Address)
            .HasMaxLength(255);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.HasOne(u => u.Cart)
            .WithOne(c => c.User)
            .HasForeignKey<Cart>(c => c.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ShippingAddresses)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ProductReviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
    new User { UserID = 1, FirstName = "Hager", LastName = "Mahmoud", Email = "hager.mahmoud@email.com", Phone = "01234567891", Role = "Admin", PasswordHash = "hashed_pw_1", CreatedAt = new DateTime(2024, 1, 1), Address = "Bns" },
    new User { UserID = 2, FirstName = "Abdelrhman", LastName = "Salah", Email = "abdelrhman.salah@email.com", Phone = "01234567891", Role = "Admin", PasswordHash = "hashed_pw_2", CreatedAt = new DateTime(2024, 1, 1), Address = "Bns" },
    new User { UserID = 3, FirstName = "Osama", LastName = "Tarek", Email = "osama.tarek@email.com", Phone = "01234567891", Role = "Admin", PasswordHash = "hashed_pw_3", CreatedAt = new DateTime(2024, 2, 1), Address = "Bns" }
    );
    }
}
