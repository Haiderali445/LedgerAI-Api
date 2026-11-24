using Microsoft.AspNetCore.Mvc;
using LedgerAI.Api.Data.Dtos.Catalog;
using LedgerAI.Api.Data.Dtos.Common;
using LedgerAI.Api.Modules.Catalog;

namespace LedgerAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(ICatalogService catalogService, ILogger<CatalogController> logger)
    {
        _catalogService = catalogService;
        _logger = logger;
    }

    [HttpGet("products")]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<ProductDto>>>> GetProducts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving catalog products");
        var result = await _catalogService.GetProductsAsync(cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogWarning("Catalog product retrieval failed");
            return BadRequest(ApiResponseDto<IReadOnlyList<ProductDto>>.Fail(result.Error!));
        }

        return Ok(ApiResponseDto<IReadOnlyList<ProductDto>>.Ok(Array.Empty<ProductDto>()));
    }
}
