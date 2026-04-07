using AiMeetingSummariser.Domain.Common;

namespace AiMeetingSummariser.Domain.Entities;

public class Transcript : BaseEntity
{
    public Guid MeetingId { get; set; }
    public string FullText { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    
    public Meeting? Meeting { get; set; }
}
