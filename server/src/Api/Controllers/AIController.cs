using MediatR;
using Microsoft.AspNetCore.Mvc;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Middleware;
using AiMeetingSummariser.Api.Application.Features.AI.ProcessMeeting;
using AiMeetingSummariser.Api.Application.Features.AI.ChatWithMeeting;

namespace AiMeetingSummariser.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(JwtAuthFilter))]
public class AIController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AIController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("meetings/{meetingId:guid}/process")]
    public async Task<ActionResult<ResponseWrapper<bool>>> ProcessMeeting(Guid meetingId)
    {
        return Ok(await _mediator.Send(new ProcessMeetingCommand(meetingId)));
    }
    
    [HttpPost("meetings/{meetingId:guid}/chat")]
    public async Task<ActionResult<ResponseWrapper<ChatMessageDto>>> ChatWithMeeting(
        Guid meetingId, 
        [FromBody] ChatRequest request)
    {
        return Ok(await _mediator.Send(new ChatWithMeetingCommand(meetingId, request.Message)));
    }
    
    [HttpGet("meetings/{meetingId:guid}/chat")]
    public async Task<ActionResult<ResponseWrapper<List<ChatMessageDto>>>> GetChatHistory(Guid meetingId)
    {
        return Ok(await _mediator.Send(new GetChatHistoryQuery(meetingId)));
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}
