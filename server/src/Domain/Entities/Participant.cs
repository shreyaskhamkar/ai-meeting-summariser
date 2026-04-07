using AiMeetingSummariser.Domain.Common;
using AiMeetingSummariser.Domain.Enums;

namespace AiMeetingSummariser.Domain.Entities;

public class Participant : BaseEntity
{
    public Guid MeetingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public ParticipantRole Role { get; set; } = ParticipantRole.Attendee;
    
    public Meeting? Meeting { get; set; }
}
