using SokoHub.Domain.Common.Entities;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Administration;

public class AuditEntry : Entity
{
    public Guid UserId { get; private set; }
    public string Action { get; private set; }
    public string EntityName { get; private set; }
    public Guid EntityId { get; private set; }
    public string OldValue { get; private set; }
    public string NewValue { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string IpAddress { get; private set; }

    public AuditEntry(Guid userId, string action, string entityName, Guid entityId, string oldValue, string newValue, string ipAddress)
    {
        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        OldValue = oldValue;
        NewValue = newValue;
        IpAddress = ipAddress;
        Timestamp = DateTime.UtcNow;
    }
}
