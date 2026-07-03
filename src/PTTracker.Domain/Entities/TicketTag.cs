namespace PTTracker.Domain.Entities;

/// <summary>Join entity for the many-to-many between <see cref="Ticket"/> and <see cref="Tag"/>.</summary>
public class TicketTag
{
    public Guid TicketId { get; private set; }
    public Ticket Ticket { get; private set; } = null!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = null!;

    private TicketTag() { }

    public static TicketTag Create(Guid ticketId, Guid tagId)
    {
        return new TicketTag
        {
            TicketId = ticketId,
            TagId = tagId,
        };
    }
}
