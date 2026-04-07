using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Middleware;
using AiMeetingSummariser.Api.Application.Features.Meetings.CreateMeeting;
using AiMeetingSummariser.Api.Application.Features.Meetings.GetMeetings;
using AiMeetingSummariser.Api.Application.Features.Meetings.GetMeetingById;
using AiMeetingSummariser.Api.Application.Features.Meetings.DeleteMeeting;
using AiMeetingSummariser.Api.Application.Features.Meetings.SearchMeetings;

namespace AiMeetingSummariser.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(JwtAuthFilter))]
public class MeetingsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public MeetingsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<ActionResult<ResponseWrapper<MeetingDto>>> Create([FromForm] CreateMeetingCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    [HttpGet]
    public async Task<ActionResult<ResponseWrapper<PaginatedList<MeetingDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetMeetingsQuery(page, pageSize);
        
        return Ok(await _mediator.Send(query));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResponseWrapper<MeetingDetailDto>>> GetById(Guid id)
    {
        return Ok(await _mediator.Send(new GetMeetingByIdQuery(id)));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ResponseWrapper<bool>>> Delete(Guid id)
    {
        return Ok(await _mediator.Send(new DeleteMeetingCommand(id)));
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<ResponseWrapper<List<MeetingDto>>>> Search([FromQuery] string searchTerm)
    {
        return Ok(await _mediator.Send(new SearchMeetingsQuery(searchTerm)));
    }
}
