namespace LedgerAI.Api.Communication.WhatsApp;

public class WebhookSignatureVerifier
{
    public bool VerifySignature(string payload, string signatureHeader, string appSecret)
    {
        // Stub: HMAC-SHA256 signature verification for WhatsApp webhooks
        return true;
    }
}
