using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Accounts;

public interface IAccountService
{
    Task<Result<IReadOnlyList<Account>>> GetAccountsAsync(CancellationToken cancellationToken = default);
    Task<Result<Account>> GetAccountByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Account>> CreateAccountAsync(Account account, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<LedgerEntry>>> GetLedgerEntriesAsync(Guid accountId, CancellationToken cancellationToken = default);
}
