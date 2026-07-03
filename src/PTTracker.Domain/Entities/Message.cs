using PTTracker.Domain.Common;
using PTTracker.Domain.Enums;

namespace PTTracker.Domain.Entities;

public class Message : Entity
{
    private readonly List<Attachment> _attachments = new();

    public Guid TicketId { get; private set; }
    public Ticket Ticket { get; private set; } = null!;

    public SenderType SenderType { get; private set; }

    // Set when the sender is an agent; null for customer/system messages.
    public Guid? AuthorUserId { get; private set; }
    public User? AuthorUser { get; private set; }

    public string Body { get; private set; } = null!;

    // Sanitized HTML body (preserves tables/formatting from the email); null when none.
    public string? BodyHtml { get; private set; }

    // Email provenance / idempotency fields (null/empty for manually created messages).
    public string? MessageIdHeader { get; private set; }
    public string? InReplyTo { get; private set; }
    public string ContentHash { get; private set; } = string.Empty;

    // The original send time (email Date header); falls back to ingestion time.
    public DateTime SentAt { get; private set; }

    public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();

    private Message() { }

    public static Message Create(
        Guid ticketId,
        SenderType senderType,
        string body,
        Guid? authorUserId = null)
    {
        return new Message
        {
            TicketId = ticketId,
            SenderType = senderType,
            Body = body,
            AuthorUserId = authorUserId,
            SentAt = DateTime.UtcNow,
        };
    }

    public static Message CreateFromEmail(
        Guid ticketId,
        SenderType senderType,
        string body,
        string? bodyHtml,
        string contentHash,
        string? messageIdHeader,
        string? inReplyTo,
        DateTime sentAt)
    {
        return new Message
        {
            TicketId = ticketId,
            SenderType = senderType,
            Body = body,
            BodyHtml = bodyHtml,
            ContentHash = contentHash,
            MessageIdHeader = messageIdHeader,
            InReplyTo = inReplyTo,
            SentAt = sentAt,
        };
    }

    public void AddAttachment(Attachment attachment) => _attachments.Add(attachment);
}
