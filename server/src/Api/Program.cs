using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AiMeetingSummariser.Api.Infrastructure.Persistence;
using AiMeetingSummariser.Api.Infrastructure.Services;
using AiMeetingSummariser.Api.Application.Interfaces;
using AiMeetingSummariser.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.WriteTo.Console();
    configuration.WriteTo.Debug();
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "AiMeetingSummariser",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "AiMeetingSummariser",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyHereThatIsAtLeast32Chars!"))
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IActionFilter, JwtAuthFilter>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<ITranscriptionService, MockTranscriptionService>();
builder.Services.AddScoped<IMeetingSummaryService, MockMeetingSummaryService>();
builder.Services.AddScoped<IActionItemExtractionService, MockActionItemExtractionService>();
builder.Services.AddScoped<IDecisionExtractionService, MockDecisionExtractionService>();
builder.Services.AddScoped<IMeetingChatService, MockMeetingChatService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "Uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
