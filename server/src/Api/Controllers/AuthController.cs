using MediatR;
using Microsoft.AspNetCore.Mvc;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Middleware;
using AiMeetingSummariser.Api.Application.Features.Auth.Register;
using AiMeetingSummariser.Api.Application.Features.Auth.Login;
using AiMeetingSummariser.Api.Application.Features.Auth.GetCurrentUser;

namespace AiMeetingSummariser.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<ResponseWrapper<AuthResponseDto>>> Register([FromBody] RegisterCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<ResponseWrapper<AuthResponseDto>>> Login([FromBody] LoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    [HttpGet("me")]
    [ServiceFilter(typeof(JwtAuthFilter))]
    public async Task<ActionResult<ResponseWrapper<UserDto>>> GetCurrentUser()
    {
        return Ok(await _mediator.Send(new GetCurrentUserQuery()));
    }
}
