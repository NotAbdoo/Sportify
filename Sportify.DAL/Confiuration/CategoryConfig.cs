using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.CategoryID);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Description)
            .HasColumnType("nvarchar(max)");

        // Self-referencing: Category → SubCategories
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryID)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasData(
            // Root Categories
            new Category { CategoryID = 1, Name = "Men", Description = "All types items for men", ParentCategoryID = null },
            new Category { CategoryID = 2, Name = "Women's", Description = "All types of items for women", ParentCategoryID = null },
            new Category { CategoryID = 3, Name = "Equipments", Description = "Equipments for sports", ParentCategoryID = null },
            new Category { CategoryID = 4, Name = "Sports", Description = "Sports", ParentCategoryID = null },

            // Sub-categories → Men
            new Category { CategoryID = 5, Name = "Men's Clothing", Description = "Clothing for men.", ParentCategoryID = 1 },
            new Category { CategoryID = 6, Name = "Men's Footwear", Description = "Footwear for men.", ParentCategoryID = 1 },

            // Sub-categories → Women
            new Category { CategoryID = 7, Name = "Women's Clothing", Description = "Clothing for women.", ParentCategoryID = 2 },
            new Category { CategoryID = 8, Name = "Women's Footwear", Description = "Footwear for women.", ParentCategoryID = 2 },

            // Sub-categories → Accessories
            new Category { CategoryID = 9, Name = "Headwears", Description = "Earphones and  others", ParentCategoryID = 3 },
            new Category { CategoryID = 10, Name = "Others", Description = "Things like handgrips", ParentCategoryID = 3 },

            // Sub-categories → Sports
            new Category { CategoryID = 11, Name = "FootBall", Description = "FootBall items", ParentCategoryID = 4 },
            new Category { CategoryID = 12, Name = "BasketBall", Description = "BasketBall items", ParentCategoryID = 4 },
            new Category { CategoryID = 13, Name = "Tennis", Description = "Tennis items", ParentCategoryID = 4 },
            new Category { CategoryID = 14, Name = "Running", Description = "Running items", ParentCategoryID = 4 },
            new Category { CategoryID = 15, Name = "Gym", Description = "Gym items", ParentCategoryID = 4 }
        );
    }
}