using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Invoicing;

public class InvoiceService : IInvoiceService
{
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(ILogger<InvoiceService> logger)
    {
        _logger = logger;
    }

    public Task<Result<IReadOnlyList<Invoice>>> GetInvoicesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<Invoice>>(Array.Empty<Invoice>()));
    }

    public Task<Result<Invoice>> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Invoice>("Not implemented."));
    }

    public Task<Result<Invoice>> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Invoice>("Not implemented."));
    }
}
