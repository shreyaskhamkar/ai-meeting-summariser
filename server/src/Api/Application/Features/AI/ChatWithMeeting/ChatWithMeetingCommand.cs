using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Application.Interfaces;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using AiMeetingSummariser.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AiMeetingSummariser.Api.Application.Features.AI.ChatWithMeeting;

public record ChatWithMeetingCommand(Guid MeetingId, string Message) : IRequest<ResponseWrapper<ChatMessageDto>>;

public class ChatWithMeetingCommandValidator : AbstractValidator<ChatWithMeetingCommand>
{
    public ChatWithMeetingCommandValidator()
    {
        RuleFor(x => x.MeetingId)
            .NotEmpty()
            .WithMessage("Meeting ID is required");
        
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required")
            .MaximumLength(2000);
    }
}

public class ChatWithMeetingHandler : IRequestHandler<ChatWithMeetingCommand, ResponseWrapper<ChatMessageDto>>
{
    private readonly AppDbContext _context;
    private readonly IMeetingChatService _chatService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ChatWithMeetingHandler(AppDbContext context, IMeetingChatService chatService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _chatService = chatService;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<ChatMessageDto>> Handle(ChatWithMeetingCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<ChatMessageDto>.ErrorResponse("User not authenticated");
        }
        
        var meeting = await _context.Meetings
            .Where(m => m.Id == request.MeetingId && m.CreatedByUserId == userId)
            .Include(m => m.Transcript)
            .Include(m => m.Summary)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (meeting == null)
        {
            return ResponseWrapper<ChatMessageDto>.ErrorResponse("Meeting not found");
        }
        
        var userMessage = new ChatMessage
        {
            MeetingId = meeting.Id,
            Role = Domain.Enums.ChatRole.User,
            Message = request.Message
        };
        _context.ChatMessages.Add(userMessage);
        
        var transcriptText = meeting.Transcript?.FullText ?? "";
        var summaryText = meeting.Summary?.DetailedSummary ?? "";
        
        var aiResponse = await _chatService.ProcessChatMessageAsync(request.Message, transcriptText, summaryText);
        
        var assistantMessage = new ChatMessage
        {
            MeetingId = meeting.Id,
            Role = Domain.Enums.ChatRole.Assistant,
            Message = aiResponse
        };
        _context.ChatMessages.Add(assistantMessage);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        var responseDto = new ChatMessageDto
        {
            Id = assistantMessage.Id,
            Role = assistantMessage.Role.ToString(),
            Message = assistantMessage.Message,
            CreatedAt = assistantMessage.CreatedAt
        };
        
        return ResponseWrapper<ChatMessageDto>.SuccessResponse(responseDto, "Response generated successfully");
    }
}

public record GetChatHistoryQuery(Guid MeetingId) : IRequest<ResponseWrapper<List<ChatMessageDto>>>;

public class GetChatHistoryHandler : IRequestHandler<GetChatHistoryQuery, ResponseWrapper<List<ChatMessageDto>>>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public GetChatHistoryHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<List<ChatMessageDto>>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<List<ChatMessageDto>>.ErrorResponse("User not authenticated");
        }
        
        var meeting = await _context.Meetings
            .FirstOrDefaultAsync(m => m.Id == request.MeetingId && m.CreatedByUserId == userId, cancellationToken);
        
        if (meeting == null)
        {
            return ResponseWrapper<List<ChatMessageDto>>.ErrorResponse("Meeting not found");
        }
        
        var chatMessages = await _context.ChatMessages
            .Where(cm => cm.MeetingId == request.MeetingId)
            .OrderBy(cm => cm.CreatedAt)
            .ToListAsync(cancellationToken);
        
        var chatMessageDtos = chatMessages.Select(cm => new ChatMessageDto
        {
            Id = cm.Id,
            Role = cm.Role.ToString(),
            Message = cm.Message,
            CreatedAt = cm.CreatedAt
        }).ToList();
        
        return ResponseWrapper<List<ChatMessageDto>>.SuccessResponse(chatMessageDtos, "Chat history retrieved successfully");
    }
}