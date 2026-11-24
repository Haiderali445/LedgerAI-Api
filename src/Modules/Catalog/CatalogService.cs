using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Catalog;

public class CatalogService : ICatalogService
{
    private readonly ILogger<CatalogService> _logger;

    public CatalogService(ILogger<CatalogService> logger)
    {
        _logger = logger;
    }

    public Task<Result<IReadOnlyList<Product>>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<Product>>(Array.Empty<Product>()));
    }

    public Task<Result<Product>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Product>("Not implemented."));
    }

    public Task<Result<Product>> CreateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Product>("Not implemented."));
    }

    public Task<Result<IReadOnlyList<Category>>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<Category>>(Array.Empty<Category>()));
    }

    public Task<Result<IReadOnlyList<UnitOfMeasure>>> GetUnitsOfMeasureAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<UnitOfMeasure>>(Array.Empty<UnitOfMeasure>()));
    }
}
