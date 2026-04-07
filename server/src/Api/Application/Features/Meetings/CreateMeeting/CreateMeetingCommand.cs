using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using AiMeetingSummariser.Api.Infrastructure.Services;
using AiMeetingSummariser.Domain.Entities;
using AiMeetingSummariser.Domain.Enums;

namespace AiMeetingSummariser.Api.Application.Features.Meetings.CreateMeeting;

public record CreateMeetingCommand : IRequest<ResponseWrapper<MeetingDto>>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? MeetingDate { get; set; }
    public IFormFile? File { get; set; }
}

public class CreateMeetingCommandValidator : AbstractValidator<CreateMeetingCommand>
{
    public CreateMeetingCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(500);
    }
}

public class CreateMeetingHandler : IRequestHandler<CreateMeetingCommand, ResponseWrapper<MeetingDto>>
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CreateMeetingHandler(AppDbContext context, IFileStorageService fileStorageService, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<MeetingDto>> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<MeetingDto>.ErrorResponse("User not authenticated");
        }
        
        var meeting = new Meeting
        {
            Title = request.Title,
            Description = request.Description,
            MeetingDate = request.MeetingDate,
            CreatedByUserId = userId,
            Status = MeetingStatus.Uploaded
        };
        
        if (request.File != null && request.File.Length > 0)
        {
            using var stream = request.File.OpenReadStream();
            var filePath = await _fileStorageService.SaveFileAsync(stream, request.File.FileName);
            meeting.FileUrl = filePath;
            meeting.OriginalFileName = request.File.FileName;
        }
        
        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync(cancellationToken);
        
        var meetingDto = new MeetingDto
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
            ActionItemCount = 0,
            DecisionCount = 0
        };
        
        return ResponseWrapper<MeetingDto>.SuccessResponse(meetingDto, "Meeting created successfully");
    }
}