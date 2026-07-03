using ErrorOr;
using MediatR;
using PTTracker.Contracts.Tickets;

namespace PTTracker.Application.Tickets.Commands.CreateManualTicket;

public record CreateManualTicketCommand(
    string CustomerName,
    string CustomerEmail,
    string Subject,
    string Description,
    string Priority,
    string? DescriptionHtml) : IRequest<ErrorOr<TicketResponse>>;
