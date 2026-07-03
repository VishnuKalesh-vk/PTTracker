using PTTracker.Domain.Common;

namespace PTTracker.Domain.Entities;

public class Tag : Entity
{
    private readonly List<TicketTag> _ticketTags = new();

    public string Name { get; private set; } = null!;
    public string? Color { get; private set; }

    public IReadOnlyCollection<TicketTag> TicketTags => _ticketTags.AsReadOnly();

    private Tag() { }

    public static Tag Create(string name, string? color = null)
    {
        return new Tag
        {
            Name = name,
            Color = color,
        };
    }
}
