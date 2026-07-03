namespace PTTracker.Contracts.Tickets;

public record TicketResponse(
    Guid Id,
    long TicketNumber,
    string Subject,
    string Description,
    string? DescriptionHtml,
    string Status,
    string Priority,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    Guid? AssignedUserId,
    string? AssignedUserName,
    string? JiraId,
    IReadOnlyList<string> Tags,
    IReadOnlyList<MessageResponse> Messages,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? ResolvedAt,
    DateTime? ClosedAt);
