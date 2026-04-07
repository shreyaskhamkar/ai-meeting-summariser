using AiMeetingSummariser.Domain.Common;

namespace AiMeetingSummariser.Domain.Entities;

public class Summary : BaseEntity
{
    public Guid MeetingId { get; set; }
    public string ShortSummary { get; set; } = string.Empty;
    public string DetailedSummary { get; set; } = string.Empty;
    public string? KeyDiscussionPoints { get; set; }
    public string? RisksOrBlockers { get; set; }
    
    public Meeting? Meeting { get; set; }
}
