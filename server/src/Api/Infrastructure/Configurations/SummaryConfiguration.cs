using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AiMeetingSummariser.Domain.Entities;

namespace AiMeetingSummariser.Api.Infrastructure.Configurations;

public class SummaryConfiguration : IEntityTypeConfiguration<Summary>
{
    public void Configure(EntityTypeBuilder<Summary> builder)
    {
        builder.ToTable("Summaries");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.ShortSummary)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(s => s.DetailedSummary)
            .IsRequired();
        
        builder.Property(s => s.KeyDiscussionPoints)
            .HasMaxLength(4000);
        
        builder.Property(s => s.RisksOrBlockers)
            .HasMaxLength(2000);
    }
}
