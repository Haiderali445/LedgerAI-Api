using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Inventory;

public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(ILogger<InventoryService> logger)
    {
        _logger = logger;
    }

    public Task<Result<IReadOnlyList<StockItem>>> GetStockItemsAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<StockItem>>(Array.Empty<StockItem>()));
    }

    public Task<Result<IReadOnlyList<StockBatch>>> GetBatchesAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<StockBatch>>(Array.Empty<StockBatch>()));
    }

    public Task<Result<IReadOnlyList<Warehouse>>> GetWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>()));
    }
}
