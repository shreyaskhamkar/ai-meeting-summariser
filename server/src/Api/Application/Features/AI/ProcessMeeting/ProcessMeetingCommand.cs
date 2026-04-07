using MediatR;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Application.Interfaces;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using AiMeetingSummariser.Domain.Entities;
using AiMeetingSummariser.Domain.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AiMeetingSummariser.Api.Application.Features.AI.ProcessMeeting;

public record ProcessMeetingCommand(Guid MeetingId) : IRequest<ResponseWrapper<bool>>;

public class ProcessMeetingHandler : IRequestHandler<ProcessMeetingCommand, ResponseWrapper<bool>>
{
    private readonly AppDbContext _context;
    private readonly ITranscriptionService _transcriptionService;
    private readonly IMeetingSummaryService _summaryService;
    private readonly IActionItemExtractionService _actionItemService;
    private readonly IDecisionExtractionService _decisionService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ProcessMeetingHandler(
        AppDbContext context,
        ITranscriptionService transcriptionService,
        IMeetingSummaryService summaryService,
        IActionItemExtractionService actionItemService,
        IDecisionExtractionService decisionService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _transcriptionService = transcriptionService;
        _summaryService = summaryService;
        _actionItemService = actionItemService;
        _decisionService = decisionService;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<bool>> Handle(ProcessMeetingCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<bool>.ErrorResponse("User not authenticated");
        }
        
        var meeting = await _context.Meetings
            .FirstOrDefaultAsync(m => m.Id == request.MeetingId && m.CreatedByUserId == userId, cancellationToken);
        
        if (meeting == null)
        {
            return ResponseWrapper<bool>.ErrorResponse("Meeting not found");
        }
        
        if (string.IsNullOrEmpty(meeting.FileUrl))
        {
            return ResponseWrapper<bool>.ErrorResponse("No audio file found for this meeting");
        }
        
        try
        {
            var transcriptionText = await _transcriptionService.TranscribeAudioAsync(
                new MemoryStream(), meeting.OriginalFileName ?? "audio");
            
            var transcript = new Transcript
            {
                MeetingId = meeting.Id,
                FullText = transcriptionText,
                Language = "en"
            };
            _context.Transcripts.Add(transcript);
            
            var (shortSummary, detailedSummary, keyPoints, risks) = await _summaryService.GenerateSummaryAsync(transcriptionText);
            
            var summary = new Summary
            {
                MeetingId = meeting.Id,
                ShortSummary = shortSummary,
                DetailedSummary = detailedSummary,
                KeyDiscussionPoints = keyPoints,
                RisksOrBlockers = risks
            };
            _context.Summaries.Add(summary);
            
            var actionItems = await _actionItemService.ExtractActionItemsAsync(transcriptionText);
            foreach (var item in actionItems)
            {
                var actionItem = new ActionItem
                {
                    MeetingId = meeting.Id,
                    Task = item.Task,
                    OwnerName = item.OwnerName,
                    Deadline = item.Deadline,
                    Priority = Enum.TryParse<ActionItemPriority>(item.Priority, out var priority) ? priority : ActionItemPriority.Medium,
                    Status = ActionItemStatus.Pending
                };
                _context.ActionItems.Add(actionItem);
            }
            
            var decisions = await _decisionService.ExtractDecisionsAsync(transcriptionText);
            foreach (var decision in decisions)
            {
                var newDecision = new Decision
                {
                    MeetingId = meeting.Id,
                    DecisionText = decision.DecisionText
                };
                _context.Decisions.Add(newDecision);
            }
            
            meeting.Status = MeetingStatus.Completed;
            await _context.SaveChangesAsync(cancellationToken);
            
            return ResponseWrapper<bool>.SuccessResponse(true, "Meeting processed successfully");
        }
        catch (Exception ex)
        {
            return ResponseWrapper<bool>.ErrorResponse($"Error processing meeting: {ex.Message}");
        }
    }
}