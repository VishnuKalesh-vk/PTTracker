using ErrorOr;
using MediatR;
using PTTracker.Contracts.Tickets;

namespace PTTracker.Application.Tickets.Commands.IngestEmail;

public record IngestEmailCommand(
    byte[] EmlContent,
    string FileName) : IRequest<ErrorOr<EmailIngestResponse>>;
