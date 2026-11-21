using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Data.Dtos.AI;

namespace LedgerAI.Api.AI.Agents;

public class IntentExtractorAgent
{
    public Task<Result<NormalizedIntentDto>> ExtractIntentAsync(string inputMessage, CancellationToken cancellationToken = default)
    {
        // Stub: AI Intent Extraction will be implemented in subsequent phases
        return Task.FromResult(Result.Failure<NormalizedIntentDto>("Not implemented."));
    }
}
