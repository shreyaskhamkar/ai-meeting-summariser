using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using AiMeetingSummariser.Domain.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AiMeetingSummariser.Api.Application.Features.ActionItems.UpdateActionItem;

public record UpdateActionItemCommand(Guid Id, string? Status = null, string? Priority = null) : IRequest<ResponseWrapper<bool>>;

public class UpdateActionItemCommandValidator : AbstractValidator<UpdateActionItemCommand>
{
    public UpdateActionItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Action item ID is required");
        
        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrEmpty(s) || Enum.TryParse<ActionItemStatus>(s, out _))
            .WithMessage("Invalid status value");
        
        RuleFor(x => x.Priority)
            .Must(p => string.IsNullOrEmpty(p) || Enum.TryParse<ActionItemPriority>(p, out _))
            .WithMessage("Invalid priority value");
    }
}

public class UpdateActionItemHandler : IRequestHandler<UpdateActionItemCommand, ResponseWrapper<bool>>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UpdateActionItemHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<bool>> Handle(UpdateActionItemCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<bool>.ErrorResponse("User not authenticated");
        }
        
        var actionItem = await _context.ActionItems
            .Include(ai => ai.Meeting)
            .FirstOrDefaultAsync(ai => ai.Id == request.Id && ai.Meeting != null && ai.Meeting.CreatedByUserId == userId, cancellationToken);
        
        if (actionItem == null)
        {
            return ResponseWrapper<bool>.ErrorResponse("Action item not found");
        }
        
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ActionItemStatus>(request.Status, out var status))
        {
            actionItem.Status = status;
        }
        
        if (!string.IsNullOrEmpty(request.Priority) && Enum.TryParse<ActionItemPriority>(request.Priority, out var priority))
        {
            actionItem.Priority = priority;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return ResponseWrapper<bool>.SuccessResponse(true, "Action item updated successfully");
    }
}