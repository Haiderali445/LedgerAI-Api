using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Core.Enums;

namespace LedgerAI.Api.Modules.Accounts;

public class Account : BaseEntity
{
    public string AccountCode { get; set; } = string.Empty;
    public AccountType AccountType { get; set; } = AccountType.Customer;
    public string? SubCategory { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CnicNo { get; set; }
    public string? PhoneNo { get; set; }
    public string? MobileNo { get; set; }
    public string? WhatsAppWamidRef { get; set; }
    public string? Email { get; set; }
    public string? NtnNo { get; set; }
    public string? StrnNo { get; set; }
    public string? IbanNo { get; set; }
    public string? BankAccountNo { get; set; }
    public string? ContactPerson { get; set; }
    public string? CpContactNo { get; set; }
    public string? Address { get; set; }
    public string? BusinessAddress { get; set; }
    public string? Remarks { get; set; }
    public decimal OpeningBalance { get; set; }
    public string? OpeningBalanceType { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal ReservedCredit { get; set; }
    public RiskTier RiskTier { get; set; } = RiskTier.Low;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public string MetadataJson { get; set; } = "{}";
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
}
