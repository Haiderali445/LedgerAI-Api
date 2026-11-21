using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Invoicing;

public interface IInvoiceService
{
    Task<Result<IReadOnlyList<Invoice>>> GetInvoicesAsync(CancellationToken cancellationToken = default);
    Task<Result<Invoice>> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Invoice>> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
