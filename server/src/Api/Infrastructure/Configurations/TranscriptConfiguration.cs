using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AiMeetingSummariser.Domain.Entities;

namespace AiMeetingSummariser.Api.Infrastructure.Configurations;

public class TranscriptConfiguration : IEntityTypeConfiguration<Transcript>
{
    public void Configure(EntityTypeBuilder<Transcript> builder)
    {
        builder.ToTable("Transcripts");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.FullText)
            .IsRequired();
        
        builder.Property(t => t.Language)
            .HasMaxLength(10)
            .HasDefaultValue("en");
    }
}
