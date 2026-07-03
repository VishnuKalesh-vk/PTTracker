namespace PTTracker.Contracts.Tickets;

public record CreateTicketRequest(
    Guid CustomerId,
    string Subject,
    string Description,
    string Priority = "Normal",
    Guid? AssignedUserId = null,
    string? DescriptionHtml = null);
