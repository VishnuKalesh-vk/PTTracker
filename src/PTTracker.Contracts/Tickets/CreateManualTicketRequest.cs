namespace PTTracker.Contracts.Tickets;

public record CreateManualTicketRequest(
    string CustomerName,
    string CustomerEmail,
    string Subject,
    string Description,
    string Priority = "Normal",
    string? DescriptionHtml = null);
