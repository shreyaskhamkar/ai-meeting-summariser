using MediatR;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AiMeetingSummariser.Api.Application.Features.Meetings.GetMeetingById;

public record GetMeetingByIdQuery(Guid Id) : IRequest<ResponseWrapper<MeetingDetailDto>>;

public class GetMeetingByIdHandler : IRequestHandler<GetMeetingByIdQuery, ResponseWrapper<MeetingDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public GetMeetingByIdHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<MeetingDetailDto>> Handle(GetMeetingByIdQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<MeetingDetailDto>.ErrorResponse("User not authenticated");
        }
        
        var meeting = await _context.Meetings
            .Where(m => m.Id == request.Id && m.CreatedByUserId == userId)
            .Include(m => m.Transcript)
            .Include(m => m.Summary)
            .Include(m => m.ActionItems)
            .Include(m => m.Decisions)
            .Include(m => m.Participants)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (meeting == null)
        {
            return ResponseWrapper<MeetingDetailDto>.ErrorResponse("Meeting not found");
        }
        
        var meetingDto = new MeetingDetailDto
        {
            Id = meeting.Id,
            Title = meeting.Title,
            Description = meeting.Description,
            FileUrl = meeting.FileUrl,
            OriginalFileName = meeting.OriginalFileName,
            DurationInMinutes = meeting.DurationInMinutes,
            Status = meeting.Status.ToString(),
            MeetingDate = meeting.MeetingDate,
            CreatedAt = meeting.CreatedAt,
            Transcript = meeting.Transcript != null ? new TranscriptDto
            {
                Id = meeting.Transcript.Id,
                FullText = meeting.Transcript.FullText,
                Language = meeting.Transcript.Language,
                CreatedAt = meeting.Transcript.CreatedAt
            } : null,
            Summary = meeting.Summary != null ? new SummaryDto
            {
                Id = meeting.Summary.Id,
                ShortSummary = meeting.Summary.ShortSummary,
                DetailedSummary = meeting.Summary.DetailedSummary,
                KeyDiscussionPoints = meeting.Summary.KeyDiscussionPoints,
                RisksOrBlockers = meeting.Summary.RisksOrBlockers,
                CreatedAt = meeting.Summary.CreatedAt
            } : null,
            ActionItems = meeting.ActionItems.Select(ai => new ActionItemDto
            {
                Id = ai.Id,
                Task = ai.Task,
                OwnerName = ai.OwnerName,
                Deadline = ai.Deadline,
                Priority = ai.Priority.ToString(),
                Status = ai.Status.ToString()
            }).ToList(),
            Decisions = meeting.Decisions.Select(d => new DecisionDto
            {
                Id = d.Id,
                DecisionText = d.DecisionText
            }).ToList(),
            Participants = meeting.Participants.Select(p => new ParticipantDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                Role = p.Role.ToString()
            }).ToList()
        };
        
        return ResponseWrapper<MeetingDetailDto>.SuccessResponse(meetingDto, "Meeting retrieved successfully");
    }
}