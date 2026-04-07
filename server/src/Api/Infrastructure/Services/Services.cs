using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AiMeetingSummariser.Api.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadsFolder;
    
    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads");
        
        if (!Directory.Exists(_uploadsFolder))
        {
            Directory.CreateDirectory(_uploadsFolder);
        }
    }
    
    public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_uploadsFolder, uniqueFileName);
        
        await using var outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream);
        
        return Path.Combine("Uploads", uniqueFileName);
    }
    
    public async Task<Stream> GetFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_environment.ContentRootPath, filePath);
        return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
    }
    
    public Task DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_environment.ContentRootPath, filePath);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        
        return Task.CompletedTask;
    }
    
    public string GetFileUrl(string filePath)
    {
        return $"/{filePath.Replace("\\", "/")}";
    }
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string GenerateToken(Guid userId, string email)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyHereThatIsAtLeast32Chars!"));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "AiMeetingSummariser",
            audience: _configuration["Jwt:Audience"] ?? "AiMeetingSummariser",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
