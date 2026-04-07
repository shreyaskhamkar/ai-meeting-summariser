using MediatR;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AiMeetingSummariser.Api.Application.Features.ActionItems.GetActionItems;

public record GetActionItemsQuery(Guid? MeetingId = null) : IRequest<ResponseWrapper<List<ActionItemDto>>>;

public class GetActionItemsHandler : IRequestHandler<GetActionItemsQuery, ResponseWrapper<List<ActionItemDto>>>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public GetActionItemsHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<List<ActionItemDto>>> Handle(GetActionItemsQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<List<ActionItemDto>>.ErrorResponse("User not authenticated");
        }
        
        var query = _context.ActionItems
            .Where(ai => ai.Meeting != null && ai.Meeting.CreatedByUserId == userId);
        
        if (request.MeetingId.HasValue)
        {
            query = query.Where(ai => ai.MeetingId == request.MeetingId.Value);
        }
        
        var actionItems = await query
            .OrderBy(ai => ai.Deadline)
            .ThenByDescending(ai => ai.Priority)
            .ToListAsync(cancellationToken);
        
        var actionItemDtos = actionItems.Select(ai => new ActionItemDto
        {
            Id = ai.Id,
            Task = ai.Task,
            OwnerName = ai.OwnerName,
            Deadline = ai.Deadline,
            Priority = ai.Priority.ToString(),
            Status = ai.Status.ToString()
        }).ToList();
        
        return ResponseWrapper<List<ActionItemDto>>.SuccessResponse(actionItemDtos, "Action items retrieved successfully");
    }
}