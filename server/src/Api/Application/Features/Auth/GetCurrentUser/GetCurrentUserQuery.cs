using MediatR;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AiMeetingSummariser.Api.Application.Features.Auth.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<ResponseWrapper<UserDto>>;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, ResponseWrapper<UserDto>>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public GetCurrentUserHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ResponseWrapper<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return ResponseWrapper<UserDto>.ErrorResponse("User not authenticated");
        }
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        if (user == null)
        {
            return ResponseWrapper<UserDto>.ErrorResponse("User not found");
        }
        
        var userDto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
        
        return ResponseWrapper<UserDto>.SuccessResponse(userDto, "User retrieved successfully");
    }
}