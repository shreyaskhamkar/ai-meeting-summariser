using AiMeetingSummariser.Domain.Common;
using AiMeetingSummariser.Domain.Enums;

namespace AiMeetingSummariser.Domain.Entities;

public class Meeting : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FileUrl { get; set; }
    public string? OriginalFileName { get; set; }
    public int? DurationInMinutes { get; set; }
    public MeetingStatus Status { get; set; } = MeetingStatus.Uploaded;
    public DateTime? MeetingDate { get; set; }
    public Guid CreatedByUserId { get; set; }
    
    public User? CreatedByUser { get; set; }
    public Transcript? Transcript { get; set; }
    public Summary? Summary { get; set; }
    public ICollection<ActionItem> ActionItems { get; set; } = new List<ActionItem>();
    public ICollection<Decision> Decisions { get; set; } = new List<Decision>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
}
