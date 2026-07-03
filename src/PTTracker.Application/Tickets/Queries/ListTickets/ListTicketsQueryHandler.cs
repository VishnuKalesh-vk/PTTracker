using ErrorOr;
using MediatR;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Common.Models;
using PTTracker.Application.Tickets.Common;
using PTTracker.Contracts.Common;
using PTTracker.Contracts.Tickets;
using PTTracker.Domain.Enums;

namespace PTTracker.Application.Tickets.Queries.ListTickets;

public class ListTicketsQueryHandler(ITicketRepository ticketRepository)
    : IRequestHandler<ListTicketsQuery, ErrorOr<PagedResponse<TicketSummaryResponse>>>
{
    public async Task<ErrorOr<PagedResponse<TicketSummaryResponse>>> Handle(
        ListTicketsQuery query,
        CancellationToken cancellationToken)
    {
        var status = string.IsNullOrWhiteSpace(query.Status)
            ? (TicketStatus?)null
            : Enum.Parse<TicketStatus>(query.Status, ignoreCase: true);

        var priority = string.IsNullOrWhiteSpace(query.Priority)
            ? (TicketPriority?)null
            : Enum.Parse<TicketPriority>(query.Priority, ignoreCase: true);

        var filter = new TicketFilter(
            query.Page,
            query.PageSize,
            status,
            priority,
            query.CustomerId,
            query.AssignedUserId,
            query.Search,
            query.SortBy,
            query.SortDescending);

        var page = await ticketRepository.GetPagedAsync(filter, cancellationToken);

        return new PagedResponse<TicketSummaryResponse>
        {
            Items = page.Items.Select(t => t.ToSummary()).ToList(),
            Page = page.Page,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount,
            TotalPages = page.TotalPages,
            HasPrevious = page.HasPrevious,
            HasNext = page.HasNext,
        };
    }
}
