using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AiMeetingSummariser.Domain.Entities;
using AiMeetingSummariser.Domain.Enums;

namespace AiMeetingSummariser.Api.Infrastructure.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.ToTable("Meetings");
        
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(m => m.Description)
            .HasMaxLength(2000);
        
        builder.Property(m => m.FileUrl)
            .HasMaxLength(1000);
        
        builder.Property(m => m.OriginalFileName)
            .HasMaxLength(500);
        
        builder.Property(m => m.Status)
            .HasConversion<int>()
            .HasDefaultValue(MeetingStatus.Uploaded);
        
        builder.HasOne(m => m.CreatedByUser)
            .WithMany(u => u.Meetings)
            .HasForeignKey(m => m.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(m => m.Transcript)
            .WithOne(t => t.Meeting)
            .HasForeignKey<Transcript>(t => t.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(m => m.Summary)
            .WithOne(s => s.Meeting)
            .HasForeignKey<Summary>(s => s.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(m => m.ActionItems)
            .WithOne(ai => ai.Meeting)
            .HasForeignKey(ai => ai.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(m => m.Decisions)
            .WithOne(d => d.Meeting)
            .HasForeignKey(d => d.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(m => m.ChatMessages)
            .WithOne(cm => cm.Meeting)
            .HasForeignKey(cm => cm.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(m => m.Participants)
            .WithOne(p => p.Meeting)
            .HasForeignKey(p => p.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
