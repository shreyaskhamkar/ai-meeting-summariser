using MediatR;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AiMeetingSummariser.Api.Application.Features.Meetings.DeleteMeeting;

public record DeleteMeetingCommand(Guid Id) : IRequest<ResponseWrapper<bool>>;

public class DeleteMeetingHandler : IRequestHandler<DeleteMeetingCommand, ResponseWrapper<bool>>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public DeleteMeetingHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<bool>> Handle(DeleteMeetingCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<bool>.ErrorResponse("User not authenticated");
        }
        
        var meeting = await _context.Meetings
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.CreatedByUserId == userId, cancellationToken);
        
        if (meeting == null)
        {
            return ResponseWrapper<bool>.ErrorResponse("Meeting not found");
        }
        
        _context.Meetings.Remove(meeting);
        await _context.SaveChangesAsync(cancellationToken);
        
        return ResponseWrapper<bool>.SuccessResponse(true, "Meeting deleted successfully");
    }
}