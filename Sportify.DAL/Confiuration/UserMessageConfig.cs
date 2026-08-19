using Sportify.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserMessageConfig : IEntityTypeConfiguration<UserMessage>
{
    public void Configure(EntityTypeBuilder<UserMessage> builder)
    {
        builder.HasKey(um => um.UserMessageID);

        builder.Property(um => um.MessageText)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(um => um.CreatedAt)
            .IsRequired();

        builder.HasOne(um => um.User)
            .WithMany(u => u.Messages)
            .HasForeignKey(um => um.UserID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
