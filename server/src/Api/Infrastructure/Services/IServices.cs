namespace AiMeetingSummariser.Api.Infrastructure.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName);
    Task<Stream> GetFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
    string GetFileUrl(string filePath);
}

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email);
}
