using ErrorOr;
using MediatR;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Tickets.Common;
using PTTracker.Contracts.Tickets;
using PTTracker.Domain.Entities;
using PTTracker.Domain.Enums;
using PTTracker.Domain.Errors;

namespace PTTracker.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler(
    ITicketRepository ticketRepository,
    ICustomerRepository customerRepository,
    IUserRepository userRepository,
    IHtmlSanitizer htmlSanitizer,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTicketCommand, ErrorOr<TicketResponse>>
{
    public async Task<ErrorOr<TicketResponse>> Handle(
        CreateTicketCommand command,
        CancellationToken cancellationToken)
    {
        if (!await customerRepository.ExistsAsync(command.CustomerId, cancellationToken))
            return Errors.Customer.NotFound;

        if (command.AssignedUserId is Guid assignedUserId
            && await userRepository.GetByIdAsync(assignedUserId, cancellationToken) is null)
        {
            return Errors.User.NotFound;
        }

        var priority = Enum.Parse<TicketPriority>(command.Priority, ignoreCase: true);

        var descriptionHtml = string.IsNullOrWhiteSpace(command.DescriptionHtml)
            ? null
            : htmlSanitizer.Sanitize(command.DescriptionHtml);

        var ticket = Ticket.Create(
            command.CustomerId,
            command.Subject,
            command.Description,
            priority,
            command.AssignedUserId,
            descriptionHtml: descriptionHtml);

        await ticketRepository.AddAsync(ticket, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with navigations so the response includes customer/assignee/tags.
        var created = await ticketRepository.GetByIdAsync(ticket.Id, cancellationToken);
        return created!.ToResponse();
    }
}
