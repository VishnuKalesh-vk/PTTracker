using PTTracker.Domain.Enums;

namespace PTTracker.Application.Common.Models;

/// <summary>Parsed, validated filter/sort/paging spec passed to the ticket repository.</summary>
public record TicketFilter(
    int Page,
    int PageSize,
    TicketStatus? Status,
    TicketPriority? Priority,
    Guid? CustomerId,
    Guid? AssignedUserId,
    string? Search,
    string SortBy,
    bool SortDescending);
