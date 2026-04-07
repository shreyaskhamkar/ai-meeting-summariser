using AiMeetingSummariser.Domain.Common;
using AiMeetingSummariser.Domain.Enums;

namespace AiMeetingSummariser.Domain.Entities;

public class ActionItem : BaseEntity
{
    public Guid MeetingId { get; set; }
    public string Task { get; set; } = string.Empty;
    public string? OwnerName { get; set; }
    public DateTime? Deadline { get; set; }
    public ActionItemPriority Priority { get; set; } = ActionItemPriority.Medium;
    public ActionItemStatus Status { get; set; } = ActionItemStatus.Pending;
    
    public Meeting? Meeting { get; set; }
}
