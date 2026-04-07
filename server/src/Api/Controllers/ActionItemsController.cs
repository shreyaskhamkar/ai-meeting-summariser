using MediatR;
using Microsoft.AspNetCore.Mvc;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Middleware;
using AiMeetingSummariser.Api.Application.Features.ActionItems.GetActionItems;
using AiMeetingSummariser.Api.Application.Features.ActionItems.UpdateActionItem;

namespace AiMeetingSummariser.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(JwtAuthFilter))]
public class ActionItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ActionItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<ActionResult<ResponseWrapper<List<ActionItemDto>>>> GetAll(
        [FromQuery] Guid? meetingId)
    {
        return Ok(await _mediator.Send(new GetActionItemsQuery(meetingId)));
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ResponseWrapper<ActionItemDto>>> Update(
        Guid id, 
        [FromBody] UpdateActionItemRequest request)
    {
        var command = new UpdateActionItemCommand(
            id,
            request.Status,
            request.Priority);
        
        return Ok(await _mediator.Send(command));
    }
}

public class UpdateActionItemRequest
{
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? OwnerName { get; set; }
    public DateTime? Deadline { get; set; }
}
