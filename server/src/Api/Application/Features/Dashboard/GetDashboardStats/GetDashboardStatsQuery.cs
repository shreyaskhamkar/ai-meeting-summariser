using MediatR;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using AiMeetingSummariser.Domain.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AiMeetingSummariser.Api.Application.Features.Dashboard.GetDashboardStats;

public record GetDashboardStatsQuery : IRequest<ResponseWrapper<DashboardStatsDto>>;

public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, ResponseWrapper<DashboardStatsDto>>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public GetDashboardStatsHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<DashboardStatsDto>.ErrorResponse("User not authenticated");
        }
        
        var meetings = await _context.Meetings
            .Where(m => m.CreatedByUserId == userId)
            .Include(m => m.ActionItems)
            .ToListAsync(cancellationToken);
        
        var actionItems = await _context.ActionItems
            .Where(ai => ai.Meeting != null && ai.Meeting.CreatedByUserId == userId)
            .ToListAsync(cancellationToken);
        
        var recentMeetings = meetings
            .OrderByDescending(m => m.CreatedAt)
            .Take(5)
            .Select(m => new MeetingDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                FileUrl = m.FileUrl,
                OriginalFileName = m.OriginalFileName,
                DurationInMinutes = m.DurationInMinutes,
                Status = m.Status.ToString(),
                MeetingDate = m.MeetingDate,
                CreatedAt = m.CreatedAt,
                Summary = m.Summary != null ? m.Summary.ShortSummary : null,
                ActionItemCount = m.ActionItems.Count,
                DecisionCount = m.Decisions.Count
            }).ToList();
        
        var upcomingDeadlines = actionItems
            .Where(ai => ai.Deadline != null && ai.Deadline > DateTime.UtcNow && ai.Status != ActionItemStatus.Completed)
            .OrderBy(ai => ai.Deadline)
            .Take(5)
            .Select(ai => new ActionItemDto
            {
                Id = ai.Id,
                Task = ai.Task,
                OwnerName = ai.OwnerName,
                Deadline = ai.Deadline,
                Priority = ai.Priority.ToString(),
                Status = ai.Status.ToString()
            }).ToList();
        
        var stats = new DashboardStatsDto
        {
            TotalMeetings = meetings.Count,
            CompletedMeetings = meetings.Count(m => m.Status == MeetingStatus.Completed),
            PendingActionItems = actionItems.Count(ai => ai.Status == ActionItemStatus.Pending),
            CompletedActionItems = actionItems.Count(ai => ai.Status == ActionItemStatus.Completed),
            RecentMeetings = recentMeetings,
            UpcomingDeadlines = upcomingDeadlines
        };
        
        return ResponseWrapper<DashboardStatsDto>.SuccessResponse(stats, "Dashboard stats retrieved successfully");
    }
}