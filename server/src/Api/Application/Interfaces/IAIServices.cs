namespace AiMeetingSummariser.Api.Application.Interfaces;

public interface ITranscriptionService
{
    Task<string> TranscribeAudioAsync(Stream audioStream, string fileName);
}

public interface IMeetingSummaryService
{
    Task<(string ShortSummary, string DetailedSummary, string KeyPoints, string Risks)> GenerateSummaryAsync(string transcriptText);
}

public interface IActionItemExtractionService
{
    Task<List<AiMeetingSummariser.Api.Application.DTOs.ActionItemDto>> ExtractActionItemsAsync(string transcriptText);
}

public interface IDecisionExtractionService
{
    Task<List<AiMeetingSummariser.Api.Application.DTOs.DecisionDto>> ExtractDecisionsAsync(string transcriptText);
}

public interface IMeetingChatService
{
    Task<string> ProcessChatMessageAsync(string userMessage, string transcriptText, string summaryText);
}
