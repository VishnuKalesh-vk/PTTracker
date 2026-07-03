using ErrorOr;
using MediatR;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Domain.Errors;

namespace PTTracker.Application.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandHandler(
    ITicketRepository ticketRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteTicketCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteTicketCommand command,
        CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(command.Id, cancellationToken);
        if (ticket is null)
            return Errors.Ticket.NotFound;

        ticketRepository.Remove(ticket);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
