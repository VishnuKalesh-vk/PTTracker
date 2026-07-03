namespace PTTracker.Contracts.Tickets;

/// <summary>Result of ingesting an .eml file. Outcome is "Created", "Appended", or "Duplicate".</summary>
public record EmailIngestResponse(
    string Outcome,
    TicketResponse Ticket);
