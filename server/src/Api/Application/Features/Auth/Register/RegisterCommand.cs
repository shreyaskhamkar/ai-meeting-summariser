using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using AiMeetingSummariser.Api.Application.DTOs;
using AiMeetingSummariser.Api.Application.Common;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using AiMeetingSummariser.Api.Infrastructure.Services;
using AiMeetingSummariser.Domain.Entities;
using BCrypt.Net;

namespace AiMeetingSummariser.Api.Application.Features.Auth.Register;

public record RegisterCommand : IRequest<ResponseWrapper<AuthResponseDto>>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MaximumLength(200);
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters");
    }
}

public class RegisterHandler : IRequestHandler<RegisterCommand, ResponseWrapper<AuthResponseDto>>
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    
    public RegisterHandler(AppDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }
    
    public async Task<ResponseWrapper<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            return ResponseWrapper<AuthResponseDto>.ErrorResponse("Email already registered");
        }
        
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        
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
        
        return ResponseWrapper<AuthResponseDto>.SuccessResponse(response, "Registration successful");
    }
}