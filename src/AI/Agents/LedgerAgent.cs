using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Data.Dtos.Accounts;
using LedgerAI.Api.Data.Dtos.AI;

namespace LedgerAI.Api.AI.Agents;

public class LedgerAgent
{
    public Task<Result<AccountDto>> InquireAccountAsync(NormalizedIntentDto intent, CancellationToken cancellationToken = default)
    {
        // Stub: AI Ledger Inquiry will be implemented in subsequent phases
        return Task.FromResult(Result.Failure<AccountDto>("Not implemented."));
    }
}
