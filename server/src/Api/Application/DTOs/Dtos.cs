namespace AiMeetingSummariser.Api.Application.DTOs;

public class ActionItemDto
{
    public Guid Id { get; set; }
    public string Task { get; set; } = string.Empty;
    public string? OwnerName { get; set; }
    public DateTime? Deadline { get; set; }
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Pending";
}

public class DecisionDto
{
    public Guid Id { get; set; }
    public string DecisionText { get; set; } = string.Empty;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class MeetingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FileUrl { get; set; }
    public string? OriginalFileName { get; set; }
    public int? DurationInMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? MeetingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Summary { get; set; }
    public int ActionItemCount { get; set; }
    public int DecisionCount { get; set; }
}

public class MeetingDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FileUrl { get; set; }
    public string? OriginalFileName { get; set; }
    public int? DurationInMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? MeetingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public TranscriptDto? Transcript { get; set; }
    public SummaryDto? Summary { get; set; }
    public List<ActionItemDto> ActionItems { get; set; } = new();
    public List<DecisionDto> Decisions { get; set; } = new();
    public List<ParticipantDto> Participants { get; set; } = new();
}

public class TranscriptDto
{
    public Guid Id { get; set; }
    public string FullText { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public DateTime CreatedAt { get; set; }
}

public class SummaryDto
{
    public Guid Id { get; set; }
    public string ShortSummary { get; set; } = string.Empty;
    public string DetailedSummary { get; set; } = string.Empty;
    public string? KeyDiscussionPoints { get; set; }
    public string? RisksOrBlockers { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ParticipantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DashboardStatsDto
{
    public int TotalMeetings { get; set; }
    public int CompletedMeetings { get; set; }
    public int PendingActionItems { get; set; }
    public int CompletedActionItems { get; set; }
    public List<MeetingDto> RecentMeetings { get; set; } = new();
    public List<ActionItemDto> UpcomingDeadlines { get; set; } = new();
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}

public class PaginatedList<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    
    public PaginatedList(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
