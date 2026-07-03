using ErrorOr;
using MediatR;
using PTTracker.Contracts.Tickets;

namespace PTTracker.Application.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(
    Guid CustomerId,
    string Subject,
    string Description,
    string Priority,
    Guid? AssignedUserId,
    string? DescriptionHtml = null) : IRequest<ErrorOr<TicketResponse>>;
