using ErrorOr;
using MediatR;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Tickets.Common;
using PTTracker.Contracts.Tickets;
using PTTracker.Domain.Errors;

namespace PTTracker.Application.Tickets.Commands.SetJiraId;

public class SetJiraIdCommandHandler(
    ITicketRepository ticketRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SetJiraIdCommand, ErrorOr<TicketResponse>>
{
    public async Task<ErrorOr<TicketResponse>> Handle(
        SetJiraIdCommand command,
        CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(command.Id, cancellationToken);
        if (ticket is null)
            return Errors.Ticket.NotFound;

        ticket.SetJiraId(command.JiraId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await ticketRepository.GetByIdAsync(ticket.Id, cancellationToken);
        return updated!.ToResponse();
    }
}
