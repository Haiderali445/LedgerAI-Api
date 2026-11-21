using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Data.Dtos.AI;
using LedgerAI.Api.Data.Dtos.Orders;

namespace LedgerAI.Api.AI.Agents;

public class OrderAgent
{
    public Task<Result<OrderDto>> ProcessOrderIntentAsync(NormalizedIntentDto intent, CancellationToken cancellationToken = default)
    {
        // Stub: AI Order parsing and validation will be implemented in subsequent phases
        return Task.FromResult(Result.Failure<OrderDto>("Not implemented."));
    }
}
