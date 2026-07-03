using ErrorOr;
using MediatR;
using PTTracker.Contracts.Tickets;

namespace PTTracker.Application.Tickets.Queries.GetTicketById;

public record GetTicketByIdQuery(Guid Id) : IRequest<ErrorOr<TicketResponse>>;
