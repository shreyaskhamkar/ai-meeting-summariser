using AiMeetingSummariser.Api.Application.Interfaces;

namespace AiMeetingSummariser.Api.Infrastructure.Services;

public class MockTranscriptionService : ITranscriptionService
{
    public Task<string> TranscribeAudioAsync(Stream audioStream, string fileName)
    {
        return Task.FromResult(@"Good morning everyone, let's start our weekly product sync. Today we'll be discussing the Q2 roadmap and some technical decisions we need to make.

John: I've been thinking about our architecture. I think we should consider moving to microservices.

Sarah: That's a big change. What are the main drivers for this?

John: Mainly scalability and team autonomy. The monolithic codebase is getting harder to maintain.

Sarah: I agree. We should also discuss our database strategy.

Mike: Speaking of databases, I think we need to move from PostgreSQL to something more scalable.

Sarah: Let's not get ahead of ourselves. Let's focus on the immediate priorities first.

John: Alright, so our priorities should be: first, set up CI/CD pipeline, second, improve our testing coverage, and third, start the migration to microservices.

Mike: I'll take ownership of the CI/CD pipeline. I can have it set up by next week.

Sarah: Great. John, can you work on the testing coverage improvement?

John: Sure, I'll create a plan for that.

Mike: We also need to decide on our meeting schedule going forward.

Sarah: Let's meet weekly on Tuesdays at 10 AM. Does that work for everyone?

John: Works for me.

Mike: Same here.

Sarah: Perfect. Let's also assign action items to make sure things get done.

John: I'll create the action items and share them after the meeting.

Sarah: Great meeting everyone. Let's wrap up.");
    }
}

public class MockMeetingSummaryService : IMeetingSummaryService
{
    public Task<(string ShortSummary, string DetailedSummary, string KeyPoints, string Risks)> GenerateSummaryAsync(string transcriptText)
    {
        var shortSummary = "Weekly product sync discussing Q2 roadmap priorities including CI/CD pipeline setup, testing coverage improvements, and microservices architecture evaluation.";
        
        var detailedSummary = @"The team held their weekly product sync to discuss Q2 priorities. Key topics covered:

1. **Architecture Discussion**: John proposed moving to microservices for better scalability and team autonomy. The team acknowledged this is a significant change requiring careful planning.

2. **Database Strategy**: Mike raised the topic of database scalability, suggesting a move from PostgreSQL. Sarah advised focusing on immediate priorities first.

3. **Agreed Priorities**: 
   - First: Set up CI/CD pipeline (owned by Mike)
   - Second: Improve testing coverage (owned by John)
   - Third: Begin microservices migration planning

4. **Meeting Schedule**: The team agreed to meet weekly on Tuesdays at 10 AM.

5. **Action Items**: John agreed to create and share action items after the meeting.";
        
        var keyPoints = @"• CI/CD pipeline setup priority - Mike taking ownership
• Testing coverage improvement needed - John creating plan
• Microservices architecture evaluation for future
• Weekly meeting schedule confirmed for Tuesdays 10 AM
• Action items to be documented and shared";
        
        var risks = @"• Microservices migration is a major undertaking requiring significant planning
• Database change could introduce compatibility issues
• Testing improvements may delay other features
• Need to ensure team has bandwidth for all initiatives";

        return Task.FromResult((shortSummary, detailedSummary, keyPoints, risks));
    }
}

public class MockActionItemExtractionService : IActionItemExtractionService
{
    public Task<List<AiMeetingSummariser.Api.Application.DTOs.ActionItemDto>> ExtractActionItemsAsync(string transcriptText)
    {
        var actionItems = new List<AiMeetingSummariser.Api.Application.DTOs.ActionItemDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Task = "Set up CI/CD pipeline",
                OwnerName = "Mike",
                Deadline = DateTime.UtcNow.AddDays(7),
                Priority = "High",
                Status = "Pending"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Task = "Create testing coverage improvement plan",
                OwnerName = "John",
                Deadline = DateTime.UtcNow.AddDays(7),
                Priority = "High",
                Status = "Pending"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Task = "Create and share meeting action items",
                OwnerName = "John",
                Deadline = DateTime.UtcNow.AddDays(1),
                Priority = "Medium",
                Status = "Pending"
            }
        };

        return Task.FromResult(actionItems);
    }
}

public class MockDecisionExtractionService : IDecisionExtractionService
{
    public Task<List<AiMeetingSummariser.Api.Application.DTOs.DecisionDto>> ExtractDecisionsAsync(string transcriptText)
    {
        var decisions = new List<AiMeetingSummariser.Api.Application.DTOs.DecisionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                DecisionText = "Move to microservices architecture for better scalability"
            },
            new()
            {
                Id = Guid.NewGuid(),
                DecisionText = "Weekly meeting schedule: Tuesdays at 10 AM"
            },
            new()
            {
                Id = Guid.NewGuid(),
                DecisionText = "CI/CD pipeline is the first priority"
            }
        };

        return Task.FromResult(decisions);
    }
}

public class MockMeetingChatService : IMeetingChatService
{
    public Task<string> ProcessChatMessageAsync(string userMessage, string transcriptText, string summaryText)
    {
        var responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["who"] = "Based on this meeting, the key participants were John, Sarah, and Mike. Sarah seemed to be leading the meeting as the facilitator.",
            ["what"] = "The meeting covered Q2 roadmap priorities including CI/CD pipeline setup, testing coverage improvements, and microservices architecture evaluation.",
            ["when"] = "The team agreed to meet weekly on Tuesdays at 10 AM going forward.",
            ["action items"] = "The main action items are: 1) Mike will set up CI/CD pipeline, 2) John will create a testing coverage improvement plan, 3) John will create and share meeting action items.",
            ["decision"] = "Key decisions made: 1) Move to microservices architecture, 2) Weekly meeting schedule confirmed for Tuesdays 10 AM, 3) CI/CD pipeline is the first priority.",
            ["default"] = $"Based on the meeting transcript, I can tell you that this was a weekly product sync covering Q2 priorities. The team discussed architecture improvements, assigned action items, and set up a regular meeting schedule. Would you like more specific details about any of these topics?"
        };

        var lowerMessage = userMessage.ToLower();
        foreach (var key in responses.Keys)
        {
            if (lowerMessage.Contains(key))
            {
                return Task.FromResult(responses[key]);
            }
        }

        return Task.FromResult(responses["default"]);
    }
}
