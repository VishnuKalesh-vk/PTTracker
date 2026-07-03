using PTTracker.Domain.Common;
using PTTracker.Domain.Enums;

namespace PTTracker.Domain.Entities;

/// <summary>Append-only audit row recording each ticket status transition.</summary>
public class StatusHistory : Entity
{
    public Guid TicketId { get; private set; }
    public Ticket Ticket { get; private set; } = null!;

    // Null on the initial row created with the ticket.
    public TicketStatus? FromStatus { get; private set; }
    public TicketStatus ToStatus { get; private set; }

    public Guid? ChangedByUserId { get; private set; }
    public User? ChangedByUser { get; private set; }

    public string? Reason { get; private set; }
    public DateTime ChangedAt { get; private set; }

    private StatusHistory() { }

    public static StatusHistory Create(
        Guid ticketId,
        TicketStatus? fromStatus,
        TicketStatus toStatus,
        Guid? changedByUserId,
        string? reason)
    {
        return new StatusHistory
        {
            TicketId = ticketId,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ChangedByUserId = changedByUserId,
            Reason = reason,
            ChangedAt = DateTime.UtcNow,
        };
    }
}
