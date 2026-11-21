using Microsoft.AspNetCore.Mvc;
using LedgerAI.Api.Data.Dtos.Common;
using LedgerAI.Api.Data.Dtos.Inventory;
using LedgerAI.Api.Modules.Inventory;

namespace LedgerAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("warehouses")]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<WarehouseDto>>>> GetWarehouses(CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetWarehousesAsync(cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponseDto<IReadOnlyList<WarehouseDto>>.Fail(result.Error!));

        return Ok(ApiResponseDto<IReadOnlyList<WarehouseDto>>.Ok(Array.Empty<WarehouseDto>()));
    }
}
