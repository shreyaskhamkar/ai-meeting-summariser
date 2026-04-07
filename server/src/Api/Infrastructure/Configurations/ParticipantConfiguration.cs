using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AiMeetingSummariser.Domain.Entities;
using AiMeetingSummariser.Domain.Enums;

namespace AiMeetingSummariser.Api.Infrastructure.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("Participants");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.Email)
            .HasMaxLength(500);
        
        builder.Property(p => p.Role)
            .HasConversion<int>()
            .HasDefaultValue(ParticipantRole.Attendee);
    }
}
