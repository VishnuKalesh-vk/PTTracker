using ErrorOr;
using MediatR;
using PTTracker.Contracts.Tickets;

namespace PTTracker.Application.Tickets.Commands.UpdateTicket;

public record UpdateTicketCommand(
    Guid Id,
    string Subject,
    string Description,
    string Status,
    string Priority,
    Guid? AssignedUserId,
    Guid? ChangedByUserId,
    string? DescriptionHtml = null) : IRequest<ErrorOr<TicketResponse>>;
