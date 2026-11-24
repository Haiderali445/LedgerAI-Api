using Microsoft.AspNetCore.Mvc;
using LedgerAI.Api.Data.Dtos.Accounts;
using LedgerAI.Api.Data.Dtos.Common;
using LedgerAI.Api.Modules.Accounts;

namespace LedgerAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<AccountDto>>>> GetAccounts(CancellationToken cancellationToken)
    {
        var result = await _accountService.GetAccountsAsync(cancellationToken);
        if (result.IsFailure)
            return BadRequest(ApiResponseDto<IReadOnlyList<AccountDto>>.Fail(result.Error!));

        return Ok(ApiResponseDto<IReadOnlyList<AccountDto>>.Ok(Array.Empty<AccountDto>()));
    }
}
