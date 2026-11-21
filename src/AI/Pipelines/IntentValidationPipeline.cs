using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Data.Dtos.AI;

namespace LedgerAI.Api.AI.Pipelines;

public class IntentValidationPipeline
{
    public Result<NormalizedIntentDto> Validate(NormalizedIntentDto intent)
    {
        // Stub: Intent validation pipeline
        return Result.Success(intent);
    }
}
