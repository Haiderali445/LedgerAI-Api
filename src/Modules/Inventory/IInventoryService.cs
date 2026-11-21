using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Inventory;

public interface IInventoryService
{
    Task<Result<IReadOnlyList<StockItem>>> GetStockItemsAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<StockBatch>>> GetBatchesAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Warehouse>>> GetWarehousesAsync(CancellationToken cancellationToken = default);
}
