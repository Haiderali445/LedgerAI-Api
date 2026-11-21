using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Accounts;

public class AccountService : IAccountService
{
    public Task<Result<IReadOnlyList<Account>>> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<Account>>(Array.Empty<Account>()));
    }

    public Task<Result<Account>> GetAccountByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Account>("Not implemented."));
    }

    public Task<Result<Account>> CreateAccountAsync(Account account, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Account>("Not implemented."));
    }

    public Task<Result<IReadOnlyList<LedgerEntry>>> GetLedgerEntriesAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<LedgerEntry>>(Array.Empty<LedgerEntry>()));
    }
}
