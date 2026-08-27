namespace SokoHub.Domain.Modules.Compliance;

public sealed class ConsentRecord
{
    public Guid Id { get; init; }
    public Guid SubjectId { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public DateTimeOffset GrantedAt { get; init; }
}
