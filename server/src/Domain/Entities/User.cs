using AiMeetingSummariser.Domain.Common;

namespace AiMeetingSummariser.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}
