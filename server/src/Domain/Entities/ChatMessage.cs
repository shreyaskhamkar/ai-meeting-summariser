using AiMeetingSummariser.Domain.Common;
using AiMeetingSummariser.Domain.Enums;

namespace AiMeetingSummariser.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public Guid MeetingId { get; set; }
    public ChatRole Role { get; set; }
    public string Message { get; set; } = string.Empty;
    
    public Meeting? Meeting { get; set; }
}
