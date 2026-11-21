using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Orders;

public class OrderService : IOrderService
{
    public Task<Result<IReadOnlyList<Order>>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<Order>>(Array.Empty<Order>()));
    }

    public Task<Result<Order>> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Order>("Not implemented."));
    }

    public Task<Result<Order>> CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Order>("Not implemented."));
    }
}
