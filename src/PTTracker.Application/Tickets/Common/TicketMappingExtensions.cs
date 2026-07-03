using PTTracker.Contracts.Tickets;
using PTTracker.Domain.Entities;

namespace PTTracker.Application.Tickets.Common;

public static class TicketMappingExtensions
{
    public static TicketResponse ToResponse(this Ticket ticket) => new(
        ticket.Id,
        ticket.TicketNumber,
        ticket.Subject,
        ticket.Description,
        ticket.DescriptionHtml,
        ticket.Status.ToString(),
        ticket.Priority.ToString(),
        ticket.CustomerId,
        $"{ticket.Customer.FirstName} {ticket.Customer.LastName}",
        ticket.Customer.Email,
        ticket.AssignedUserId,
        ticket.AssignedUser is null ? null : $"{ticket.AssignedUser.FirstName} {ticket.AssignedUser.LastName}",
        ticket.JiraId,
        ticket.TicketTags.Select(tt => tt.Tag.Name).ToList(),
        ticket.Messages
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageResponse(
                m.Id,
                m.SenderType.ToString(),
                m.Body,
                m.BodyHtml,
                m.SentAt,
                m.Attachments
                    .Select(a => new AttachmentResponse(a.Id, a.FileName, a.ContentType, a.SizeBytes))
                    .ToList()))
            .ToList(),
        ticket.CreatedAt,
        ticket.UpdatedAt,
        ticket.ResolvedAt,
        ticket.ClosedAt);

    public static TicketSummaryResponse ToSummary(this Ticket ticket) => new(
        ticket.Id,
        ticket.TicketNumber,
        ticket.Subject,
        ticket.Status.ToString(),
        ticket.Priority.ToString(),
        $"{ticket.Customer.FirstName} {ticket.Customer.LastName}",
        ticket.AssignedUser is null ? null : $"{ticket.AssignedUser.FirstName} {ticket.AssignedUser.LastName}",
        ticket.JiraId,
        ticket.CreatedAt,
        ticket.UpdatedAt);
}
