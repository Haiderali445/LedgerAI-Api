using Microsoft.AspNetCore.Mvc;
using LedgerAI.Api.Data.Dtos.Common;
using LedgerAI.Api.Data.Dtos.Orders;
using LedgerAI.Api.Modules.Orders;

namespace LedgerAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<OrderDto>>>> GetOrders(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetOrdersAsync(cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponseDto<IReadOnlyList<OrderDto>>.Fail(result.Error!));

        return Ok(ApiResponseDto<IReadOnlyList<OrderDto>>.Ok(Array.Empty<OrderDto>()));
    }
}
