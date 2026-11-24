using Microsoft.AspNetCore.Mvc;
using LedgerAI.Api.Data.Dtos.Common;
using LedgerAI.Api.Data.Dtos.Invoicing;
using LedgerAI.Api.Modules.Invoicing;

namespace LedgerAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<InvoiceDto>>>> GetInvoices(CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GetInvoicesAsync(cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponseDto<IReadOnlyList<InvoiceDto>>.Fail(result.Error!));

        return Ok(ApiResponseDto<IReadOnlyList<InvoiceDto>>.Ok(Array.Empty<InvoiceDto>()));
    }
}
