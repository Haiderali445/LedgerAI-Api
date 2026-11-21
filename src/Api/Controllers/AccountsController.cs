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

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
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
