using MediatR;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AiMeetingSummariser.Api.Application.Features.Meetings.GetMeetings;

public record GetMeetingsQuery(int Page = 1, int PageSize = 10) : IRequest<ResponseWrapper<PaginatedList<MeetingDto>>>;

public class GetMeetingsHandler : IRequestHandler<GetMeetingsQuery, ResponseWrapper<PaginatedList<MeetingDto>>>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public GetMeetingsHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<PaginatedList<MeetingDto>>> Handle(GetMeetingsQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<PaginatedList<MeetingDto>>.ErrorResponse("User not authenticated");
        }
        
        var query = _context.Meetings
            .Where(m => m.CreatedByUserId == userId)
            .Include(m => m.ActionItems)
            .Include(m => m.Decisions)
            .OrderByDescending(m => m.CreatedAt);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var meetings = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        
        var meetingDtos = meetings.Select(m => new MeetingDto
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
        
        var paginatedList = new PaginatedList<MeetingDto>(meetingDtos, totalCount, request.Page, request.PageSize);
        
        return ResponseWrapper<PaginatedList<MeetingDto>>.SuccessResponse(paginatedList, "Meetings retrieved successfully");
    }
}