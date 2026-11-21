using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Orders;

public interface IOrderService
{
    Task<Result<IReadOnlyList<Order>>> GetOrdersAsync(CancellationToken cancellationToken = default);
    Task<Result<Order>> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Order>> CreateOrderAsync(Order order, CancellationToken cancellationToken = default);
}
