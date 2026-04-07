using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Domain.Entities;
using AiMeetingSummariser.Api.Infrastructure.Configurations;

namespace AiMeetingSummariser.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Meeting> Meetings => Set<Meeting>();
    public DbSet<Transcript> Transcripts => Set<Transcript>();
    public DbSet<Summary> Summaries => Set<Summary>();
    public DbSet<ActionItem> ActionItems => Set<ActionItem>();
    public DbSet<Decision> Decisions => Set<Decision>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<Participant> Participants => Set<Participant>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new MeetingConfiguration());
        modelBuilder.ApplyConfiguration(new TranscriptConfiguration());
        modelBuilder.ApplyConfiguration(new SummaryConfiguration());
        modelBuilder.ApplyConfiguration(new ActionItemConfiguration());
        modelBuilder.ApplyConfiguration(new DecisionConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMessageConfiguration());
        modelBuilder.ApplyConfiguration(new ParticipantConfiguration());
    }
}
