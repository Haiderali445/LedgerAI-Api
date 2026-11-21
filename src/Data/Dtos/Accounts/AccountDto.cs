using LedgerAI.Api.Core.Enums;

namespace LedgerAI.Api.Data.Dtos.Accounts;

public class AccountDto
{
    public Guid Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhoneNo { get; set; }
    public string? MobileNo { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal CreditLimit { get; set; }
    public RiskTier RiskTier { get; set; }
    public bool IsActive { get; set; }
}
