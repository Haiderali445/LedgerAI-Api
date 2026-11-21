using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Data.Dtos.AI;

namespace LedgerAI.Api.AI.Agents;

public class SupervisorAgent
{
    public Task<Result<string>> CoordinateAsync(NormalizedIntentDto intent, CancellationToken cancellationToken = default)
    {
        // Stub: AI Supervisor orchestration will be implemented in subsequent phases
        return Task.FromResult(Result.Failure<string>("Not implemented."));
    }
}
