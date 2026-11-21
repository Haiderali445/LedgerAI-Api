using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Communication.WhatsApp;

public class WhatsAppClient
{
    private readonly HttpClient _httpClient;

    public WhatsAppClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Result> SendTextMessageAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default)
    {
        // Stub: Outbound WhatsApp Cloud API sender
        return Task.FromResult(Result.Success());
    }
}
