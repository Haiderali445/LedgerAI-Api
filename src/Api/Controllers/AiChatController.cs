using Microsoft.AspNetCore.Mvc;
using LedgerAI.Api.AI.Agents;
using LedgerAI.Api.Data.Dtos.AI;
using LedgerAI.Api.Data.Dtos.Common;

namespace LedgerAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiChatController : ControllerBase
{
    private readonly IntentExtractorAgent _intentExtractorAgent;

    public AiChatController(IntentExtractorAgent intentExtractorAgent)
    {
        _intentExtractorAgent = intentExtractorAgent;
    }

    [HttpPost("message")]
    public async Task<ActionResult<ApiResponseDto<NormalizedIntentDto>>> PostMessage([FromBody] string message, CancellationToken cancellationToken)
    {
        var result = await _intentExtractorAgent.ExtractIntentAsync(message, cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponseDto<NormalizedIntentDto>.Fail(result.Error!));

        return Ok(ApiResponseDto<NormalizedIntentDto>.Ok(result.Value));
    }
}
