using ErrorOr;
using MediatR;
using PTTracker.Contracts.Common;
using PTTracker.Contracts.Tickets;

namespace PTTracker.Application.Tickets.Queries.ListTickets;

public record ListTicketsQuery(
    int Page,
    int PageSize,
    string? Status,
    string? Priority,
    Guid? CustomerId,
    Guid? AssignedUserId,
    string? Search,
    string SortBy,
    bool SortDescending) : IRequest<ErrorOr<PagedResponse<TicketSummaryResponse>>>;
