using LedgerAI.Api.Core.Common;
using LedgerAI.Api.Core.Enums;
using LedgerAI.Api.Modules.Accounts;

namespace LedgerAI.Api.Modules.Orders;

public class Order : BaseEntity
{
    public string OrderCode { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.PendingApproval;
    public PaymentTerms PaymentTerms { get; set; } = PaymentTerms.Credit;
    public decimal TotalAmount { get; set; }
    public decimal UpfrontPaidAmount { get; set; }
    public decimal? AiConfidenceScore { get; set; }
    public bool RequiresApproval { get; set; } = false;
    public string? ApprovalReason { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? RawIntentSummary { get; set; }
    public string ExecutionMetadataJson { get; set; } = "{}";
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset? CommittedAt { get; set; }

    // Navigation properties
    public Account? Account { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
