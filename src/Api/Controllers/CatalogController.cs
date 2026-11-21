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

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet("products")]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<ProductDto>>>> GetProducts(CancellationToken cancellationToken)
    {
        var result = await _catalogService.GetProductsAsync(cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponseDto<IReadOnlyList<ProductDto>>.Fail(result.Error!));

        return Ok(ApiResponseDto<IReadOnlyList<ProductDto>>.Ok(Array.Empty<ProductDto>()));
    }
}
