namespace PTTracker.Contracts.Tickets;

public record TicketSummaryResponse(
    Guid Id,
    long TicketNumber,
    string Subject,
    string Status,
    string Priority,
    string CustomerName,
    string? AssignedUserName,
    string? JiraId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
