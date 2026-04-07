using MediatR;
using Microsoft.AspNetCore.Mvc;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Middleware;
using AiMeetingSummariser.Api.Application.Features.Dashboard.GetDashboardStats;

namespace AiMeetingSummariser.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(JwtAuthFilter))]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("overview")]
    public async Task<ActionResult<ResponseWrapper<DashboardStatsDto>>> GetOverview()
    {
        return Ok(await _mediator.Send(new GetDashboardStatsQuery()));
    }
}
