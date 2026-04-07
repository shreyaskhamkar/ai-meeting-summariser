using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AiMeetingSummariser.Domain.Entities;

namespace AiMeetingSummariser.Api.Infrastructure.Configurations;

public class DecisionConfiguration : IEntityTypeConfiguration<Decision>
{
    public void Configure(EntityTypeBuilder<Decision> builder)
    {
        builder.ToTable("Decisions");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.DecisionText)
            .IsRequired()
            .HasMaxLength(2000);
    }
}
