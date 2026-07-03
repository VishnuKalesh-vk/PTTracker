namespace PTTracker.Contracts.Tickets;

/// <summary>Query-string bound parameters for listing tickets.</summary>
public class TicketQueryParameters
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? Priority { get; init; }
    public Guid? CustomerId { get; init; }
    public Guid? AssignedUserId { get; init; }
    public string? Search { get; init; }
    public string SortBy { get; init; } = "createdAt";
    public bool SortDescending { get; init; } = true;
}
