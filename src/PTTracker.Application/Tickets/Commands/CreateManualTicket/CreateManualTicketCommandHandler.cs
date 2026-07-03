using ErrorOr;
using MediatR;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Tickets.Common;
using PTTracker.Contracts.Tickets;
using PTTracker.Domain.Entities;
using PTTracker.Domain.Enums;

namespace PTTracker.Application.Tickets.Commands.CreateManualTicket;

public class CreateManualTicketCommandHandler(
    ITicketRepository ticketRepository,
    ICustomerRepository customerRepository,
    IHtmlSanitizer htmlSanitizer,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateManualTicketCommand, ErrorOr<TicketResponse>>
{
    public async Task<ErrorOr<TicketResponse>> Handle(
        CreateManualTicketCommand command,
        CancellationToken cancellationToken)
    {
        // Find the customer by email, or create one (same pattern as email ingest).
        var customer = await customerRepository.GetByEmailAsync(command.CustomerEmail, cancellationToken);
        if (customer is null)
        {
            var (firstName, lastName) = SplitName(command.CustomerName);
            customer = Customer.Create(firstName, lastName, command.CustomerEmail);
            await customerRepository.AddAsync(customer, cancellationToken);
        }

        var priority = Enum.Parse<TicketPriority>(command.Priority, ignoreCase: true);

        var descriptionHtml = string.IsNullOrWhiteSpace(command.DescriptionHtml)
            ? null
            : htmlSanitizer.Sanitize(command.DescriptionHtml);

        var ticket = Ticket.Create(
            customer.Id,
            command.Subject,
            command.Description,
            priority,
            source: SourceProvider.Manual,
            descriptionHtml: descriptionHtml);

        await ticketRepository.AddAsync(ticket, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await ticketRepository.GetByIdAsync(ticket.Id, cancellationToken);
        return created!.ToResponse();
    }

    private static (string FirstName, string LastName) SplitName(string name)
    {
        var parts = name.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 ? (parts[0], parts[1])
            : parts.Length == 1 ? (parts[0], "-")
            : ("Unknown", "-");
    }
}
