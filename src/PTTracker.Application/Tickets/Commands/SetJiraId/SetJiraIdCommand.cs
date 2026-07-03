using ErrorOr;
using MediatR;
using PTTracker.Contracts.Tickets;

namespace PTTracker.Application.Tickets.Commands.SetJiraId;

public record SetJiraIdCommand(Guid Id, string? JiraId) : IRequest<ErrorOr<TicketResponse>>;
