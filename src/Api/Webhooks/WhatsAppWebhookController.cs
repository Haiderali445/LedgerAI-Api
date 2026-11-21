using Microsoft.AspNetCore.Mvc;
using LedgerAI.Api.Communication.WhatsApp;

namespace LedgerAI.Api.Webhooks;

[ApiController]
[Route("api/webhooks/whatsapp")]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly WebhookSignatureVerifier _verifier;

    public WhatsAppWebhookController(WebhookSignatureVerifier verifier)
    {
        _verifier = verifier;
    }

    [HttpGet]
    public IActionResult VerifyWebhook(
        [FromQuery(Name = "hub.mode")] string? mode,
        [FromQuery(Name = "hub.verify_token")] string? token,
        [FromQuery(Name = "hub.challenge")] string? challenge)
    {
        // Stub: Hub challenge verification handshake
        if (mode == "subscribe" && !string.IsNullOrEmpty(token))
        {
            return Ok(challenge);
        }

        return Forbid();
    }

    [HttpPost]
    public IActionResult ReceiveWebhook([FromBody] object payload)
    {
        // Stub: Inbound message ingress
        return Ok();
    }
}
