using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AiMeetingSummariser.Domain.Entities;
using AiMeetingSummariser.Domain.Enums;

namespace AiMeetingSummariser.Api.Infrastructure.Configurations;

public class ActionItemConfiguration : IEntityTypeConfiguration<ActionItem>
{
    public void Configure(EntityTypeBuilder<ActionItem> builder)
    {
        builder.ToTable("ActionItems");
        
        builder.HasKey(ai => ai.Id);
        
        builder.Property(ai => ai.Task)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(ai => ai.OwnerName)
            .HasMaxLength(200);
        
        builder.Property(ai => ai.Priority)
            .HasConversion<int>()
            .HasDefaultValue(ActionItemPriority.Medium);
        
        builder.Property(ai => ai.Status)
            .HasConversion<int>()
            .HasDefaultValue(ActionItemStatus.Pending);
    }
}
