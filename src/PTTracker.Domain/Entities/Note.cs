using PTTracker.Domain.Common;

namespace PTTracker.Domain.Entities;

/// <summary>Internal, agent-only annotation on a ticket. Never shown to customers.</summary>
public class Note : Entity
{
    public Guid TicketId { get; private set; }
    public Ticket Ticket { get; private set; } = null!;

    public Guid AuthorUserId { get; private set; }
    public User AuthorUser { get; private set; } = null!;

    public string Body { get; private set; } = null!;

    private Note() { }

    public static Note Create(Guid ticketId, Guid authorUserId, string body)
    {
        return new Note
        {
            TicketId = ticketId,
            AuthorUserId = authorUserId,
            Body = body,
        };
    }
}
