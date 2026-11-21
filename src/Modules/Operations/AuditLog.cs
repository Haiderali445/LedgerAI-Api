using LedgerAI.Api.Core.Common;

namespace LedgerAI.Api.Modules.Operations;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string ActorSnapshot { get; set; } = "SYSTEM";
    public string ActionTaken { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public Guid? CorrelationId { get; set; }
}
