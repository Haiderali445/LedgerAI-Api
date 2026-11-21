namespace LedgerAI.Api.Modules.TenantAdmin;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantCode { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string? DomainSlug { get; set; }
    public string? WhatsAppPhoneNumberId { get; set; }
    public string? NtnNo { get; set; }
    public string? StrnNo { get; set; }
    public string CurrencyCode { get; set; } = "PKR";
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
}
