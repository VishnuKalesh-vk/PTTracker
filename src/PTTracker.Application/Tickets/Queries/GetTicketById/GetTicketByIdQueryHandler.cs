using ErrorOr;
using MediatR;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Tickets.Common;
using PTTracker.Contracts.Tickets;
using PTTracker.Domain.Errors;

namespace PTTracker.Application.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandler(ITicketRepository ticketRepository)
    : IRequestHandler<GetTicketByIdQuery, ErrorOr<TicketResponse>>
{
    public async Task<ErrorOr<TicketResponse>> Handle(
        GetTicketByIdQuery query,
        CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(query.Id, cancellationToken);
        if (ticket is null)
            return Errors.Ticket.NotFound;

        return ticket.ToResponse();
    }
}
