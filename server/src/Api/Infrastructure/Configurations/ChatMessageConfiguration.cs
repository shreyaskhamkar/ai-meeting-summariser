using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AiMeetingSummariser.Domain.Entities;
using AiMeetingSummariser.Domain.Enums;

namespace AiMeetingSummariser.Api.Infrastructure.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");
        
        builder.HasKey(cm => cm.Id);
        
        builder.Property(cm => cm.Message)
            .IsRequired();
        
        builder.Property(cm => cm.Role)
            .HasConversion<int>()
            .HasDefaultValue(ChatRole.User);
    }
}
