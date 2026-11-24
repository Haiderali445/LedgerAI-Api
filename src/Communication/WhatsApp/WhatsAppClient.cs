using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Communication.WhatsApp;

public class WhatsAppClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WhatsAppClient> _logger;

    public WhatsAppClient(HttpClient httpClient, ILogger<WhatsAppClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public Task<Result> SendTextMessageAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default)
    {
        // Stub: Outbound WhatsApp Cloud API sender
        return Task.FromResult(Result.Success());
    }
}
