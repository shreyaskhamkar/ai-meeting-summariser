using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using AiMeetingSummariser.Api.Infrastructure.Services;
using AiMeetingSummariser.Domain.Entities;
using BCrypt.Net;

namespace AiMeetingSummariser.Api.Application.Features.Auth.Login;

public record LoginCommand : IRequest<ResponseWrapper<AuthResponseDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}

public class LoginHandler : IRequestHandler<LoginCommand, ResponseWrapper<AuthResponseDto>>
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    
    public LoginHandler(AppDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }
    
    public async Task<ResponseWrapper<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ResponseWrapper<AuthResponseDto>.ErrorResponse("Invalid email or password");
        }
        
        var token = _jwtTokenService.GenerateToken(user.Id, user.Email);
        
        var response = new AuthResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            }
        };
        
        return ResponseWrapper<AuthResponseDto>.SuccessResponse(response, "Login successful");
    }
}