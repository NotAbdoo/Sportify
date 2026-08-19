using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BrandConfig : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.HasKey(b => b.BrandID);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.Description)
            .HasColumnType("TEXT");

        builder.Property(b => b.LogoURL)
            .HasColumnType("TEXT");

        builder.HasData(
            new Brand { BrandID = 1, Name = "Nike", Description = "The world’s largest sportswear brand", LogoURL = "https://tse4.mm.bing.net/th/id/OIP.xRP_6PtMGwc6UGslxaK4YAHaEK?rs=1&pid=ImgDetMain&o=7&rm=3",CreatedBy="Admin"},
            new Brand { BrandID = 2, Name = "Adidas", Description = "German sportswear company.", LogoURL = "https://tse3.mm.bing.net/th/id/OIP.aw4ynosen6elgMpjUjaUBwHaEK?rs=1&pid=ImgDetMain&o=7&rm=3",CreatedBy="Admin" },
            new Brand { BrandID = 3, Name = "Puma", Description = "German sportswear company.", LogoURL = "https://tse2.mm.bing.net/th/id/OIP.0z2kSI_ehJizOeLLUL77dQHaEK?rs=1&pid=ImgDetMain&o=7&rm=3",CreatedBy="Admin" },
            new Brand { BrandID = 4, Name = "Jordan", Description = "Sportswear", LogoURL = "https://tse3.mm.bing.net/th/id/OIP.tASbMfiMM2xaVjJfhDk3QgHaEK?rs=1&pid=ImgDetMain&o=7&rm=3",CreatedBy="Admin" },
            new Brand { BrandID = 5, Name = "Sony", Description = "Electronics manufacturer.", LogoURL = "https://tse3.mm.bing.net/th/id/OIP.w4RV1Nk4yYTvavsPKmXknwHaEK?rs=1&pid=ImgDetMain&o=7&rm=3",CreatedBy="Admin" }
        );
    }
}