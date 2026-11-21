using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.AI.Routers;

public class LlmFailoverRouter
{
    public Task<Result<string>> ExecuteWithFailoverAsync(string prompt, CancellationToken cancellationToken = default)
    {
        // Stub: LLM failover routing logic between primary (Groq) and fallback providers
        return Task.FromResult(Result.Failure<string>("Not implemented."));
    }
}
