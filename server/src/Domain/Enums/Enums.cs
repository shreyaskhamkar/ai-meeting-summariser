namespace AiMeetingSummariser.Domain.Enums;

public enum MeetingStatus
{
    Uploaded = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}

public enum ActionItemStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public enum ActionItemPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum ChatRole
{
    User = 0,
    Assistant = 1
}

public enum ParticipantRole
{
    Host = 0,
    Speaker = 1,
    Attendee = 2,
    NoteTaker = 3
}
