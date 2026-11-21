using LedgerAI.Api.Core.Enums;

namespace LedgerAI.Api.Data.Dtos.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public PaymentTerms PaymentTerms { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal UpfrontPaidAmount { get; set; }
    public decimal? AiConfidenceScore { get; set; }
    public bool RequiresApproval { get; set; }
    public string? ApprovalReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
