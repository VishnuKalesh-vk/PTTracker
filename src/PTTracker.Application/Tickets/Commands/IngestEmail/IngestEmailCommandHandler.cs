using ErrorOr;
using MediatR;
using Microsoft.Extensions.Options;
using PTTracker.Application.Common;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Common.Models;
using PTTracker.Application.Tickets.Common;
using PTTracker.Contracts.Tickets;
using PTTracker.Domain.Entities;
using PTTracker.Domain.Enums;

namespace PTTracker.Application.Tickets.Commands.IngestEmail;

public class IngestEmailCommandHandler(
    IEmailParser emailParser,
    IFileStorage fileStorage,
    ICustomerRepository customerRepository,
    ITicketRepository ticketRepository,
    IMessageRepository messageRepository,
    IUnitOfWork unitOfWork,
    IOptions<EmailIngestionOptions> options)
    : IRequestHandler<IngestEmailCommand, ErrorOr<EmailIngestResponse>>
{
    private readonly EmailIngestionOptions _options = options.Value;

    public async Task<ErrorOr<EmailIngestResponse>> Handle(
        IngestEmailCommand command,
        CancellationToken cancellationToken)
    {
        if (command.EmlContent.LongLength > _options.MaxFileSizeBytes)
            return Error.Validation("Email.TooLarge", $"File exceeds the {_options.MaxFileSizeBytes} byte limit.");

        ParsedEmail email;
        try
        {
            using var stream = new MemoryStream(command.EmlContent, writable: false);
            email = emailParser.Parse(stream);
        }
        catch (Exception ex) when (ex is FormatException or InvalidOperationException)
        {
            return Error.Validation("Email.Unparseable", $"Could not parse the .eml file: {ex.Message}");
        }

        if (string.IsNullOrWhiteSpace(email.FromEmail))
            return Error.Validation("Email.NoSender", "The email has no From address.");

        var contentHash = EmailThreading.ContentHash(command.EmlContent);

        // Customer: find by From address, auto-create if unknown.
        var customer = await customerRepository.GetByEmailAsync(email.FromEmail, cancellationToken);
        if (customer is null)
        {
            var (firstName, lastName) = SplitName(email.FromName, email.FromEmail);
            customer = Customer.Create(firstName, lastName, email.FromEmail);
            await customerRepository.AddAsync(customer, cancellationToken);
        }

        // Idempotency: same message already ingested -> return its ticket unchanged.
        var duplicateTicketId = await messageRepository.FindTicketIdByMessageAsync(
            email.MessageId, contentHash, cancellationToken);
        if (duplicateTicketId is Guid existingId)
        {
            var existing = await ticketRepository.GetByIdAsync(existingId, cancellationToken);
            return new EmailIngestResponse("Duplicate", existing!.ToResponse());
        }

        // Threading: 1) reply-chain via In-Reply-To/References, 2) subject-fallback within window.
        var ticket = await ResolveThreadTicketAsync(email, customer.Id, cancellationToken);

        var outcome = "Appended";
        if (ticket is null)
        {
            outcome = "Created";
            ticket = Ticket.Create(
                customer.Id,
                string.IsNullOrWhiteSpace(email.Subject) ? "(no subject)" : email.Subject,
                email.Body,
                TicketPriority.Normal,
                assignedUserId: null,
                source: SourceProvider.EmlUpload,
                threadKey: EmailThreading.SubjectThreadKey(email.Subject),
                descriptionHtml: email.BodyHtml);
            await ticketRepository.AddAsync(ticket, cancellationToken);
        }

        var message = Message.CreateFromEmail(
            ticket.Id,
            SenderType.Customer,
            email.Body,
            email.BodyHtml,
            contentHash,
            email.MessageId,
            email.InReplyTo,
            email.SentAt);

        await PersistAttachmentsAsync(ticket.Id, message, email.Attachments, cancellationToken);

        ticket.AddMessage(message);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var reloaded = await ticketRepository.GetByIdAsync(ticket.Id, cancellationToken);
        return new EmailIngestResponse(outcome, reloaded!.ToResponse());
    }

    private async Task<Ticket?> ResolveThreadTicketAsync(
        ParsedEmail email,
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var headerIds = email.References
            .Concat(email.InReplyTo is null ? Array.Empty<string>() : new[] { email.InReplyTo })
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        if (headerIds.Count > 0)
        {
            var ticketId = await messageRepository.FindTicketIdByHeaderIdsAsync(headerIds, cancellationToken);
            if (ticketId is Guid id)
                return await ticketRepository.GetByIdAsync(id, cancellationToken);
        }

        var since = DateTime.UtcNow.AddDays(-_options.SubjectFallbackWindowDays);
        var threadKey = EmailThreading.SubjectThreadKey(email.Subject);
        return await ticketRepository.FindByThreadAsync(customerId, threadKey, since, cancellationToken);
    }

    private async Task PersistAttachmentsAsync(
        Guid ticketId,
        Message message,
        IReadOnlyList<ParsedAttachment> attachments,
        CancellationToken cancellationToken)
    {
        foreach (var attachment in attachments)
        {
            var fileName = Path.GetFileName(attachment.FileName);
            var extension = Path.GetExtension(fileName);

            // v1: keep only spreadsheet-style attachments; skip everything else.
            if (!_options.AllowedAttachmentExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                continue;

            var key = $"{ticketId}/{message.Id}/{fileName}";
            var storedKey = await fileStorage.SaveAsync(key, attachment.Content, cancellationToken);

            message.AddAttachment(Attachment.Create(
                message.Id,
                fileName,
                attachment.ContentType,
                attachment.Content.LongLength,
                storedKey));
        }
    }

    private static (string FirstName, string LastName) SplitName(string? displayName, string email)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            var parts = displayName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], "-");
        }

        var local = email.Split('@')[0];
        return (local, "-");
    }
}
