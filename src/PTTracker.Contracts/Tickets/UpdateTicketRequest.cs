namespace PTTracker.Contracts.Tickets;

public record UpdateTicketRequest(
    string Subject,
    string Description,
    string Status,
    string Priority,
    Guid? AssignedUserId,
    string? DescriptionHtml = null);
