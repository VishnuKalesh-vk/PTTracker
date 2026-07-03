namespace PTTracker.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>Marks the entity as modified by bumping <see cref="UpdatedAt"/>.</summary>
    protected void Touch() => UpdatedAt = DateTime.UtcNow;
}
