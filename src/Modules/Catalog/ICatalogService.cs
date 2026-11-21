using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Catalog;

public interface ICatalogService
{
    Task<Result<IReadOnlyList<Product>>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<Result<Product>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Product>> CreateProductAsync(Product product, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Category>>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<UnitOfMeasure>>> GetUnitsOfMeasureAsync(CancellationToken cancellationToken = default);
}
