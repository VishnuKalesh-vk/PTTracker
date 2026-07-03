using ErrorOr;
using MediatR;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Tickets.Common;
using PTTracker.Contracts.Tickets;
using PTTracker.Domain.Enums;
using PTTracker.Domain.Errors;

namespace PTTracker.Application.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandHandler(
    ITicketRepository ticketRepository,
    IUserRepository userRepository,
    IHtmlSanitizer htmlSanitizer,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateTicketCommand, ErrorOr<TicketResponse>>
{
    public async Task<ErrorOr<TicketResponse>> Handle(
        UpdateTicketCommand command,
        CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(command.Id, cancellationToken);
        if (ticket is null)
            return Errors.Ticket.NotFound;

        if (command.AssignedUserId is Guid assignedUserId
            && await userRepository.GetByIdAsync(assignedUserId, cancellationToken) is null)
        {
            return Errors.User.NotFound;
        }

        var descriptionHtml = string.IsNullOrWhiteSpace(command.DescriptionHtml)
            ? null
            : htmlSanitizer.Sanitize(command.DescriptionHtml);

        ticket.UpdateDetails(command.Subject, command.Description, descriptionHtml);
        ticket.ChangePriority(Enum.Parse<TicketPriority>(command.Priority, ignoreCase: true));
        ticket.ChangeStatus(
            Enum.Parse<TicketStatus>(command.Status, ignoreCase: true),
            command.ChangedByUserId);
        ticket.Assign(command.AssignedUserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await ticketRepository.GetByIdAsync(ticket.Id, cancellationToken);
        return updated!.ToResponse();
    }
}
