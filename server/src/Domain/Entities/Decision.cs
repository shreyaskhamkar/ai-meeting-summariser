using AiMeetingSummariser.Domain.Common;

namespace AiMeetingSummariser.Domain.Entities;

public class Decision : BaseEntity
{
    public Guid MeetingId { get; set; }
    public string DecisionText { get; set; } = string.Empty;
    
    public Meeting? Meeting { get; set; }
}
