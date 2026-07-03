using ErrorOr;
using MediatR;

namespace PTTracker.Application.Tickets.Commands.DeleteTicket;

public record DeleteTicketCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;
