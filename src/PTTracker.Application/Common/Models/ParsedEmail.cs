namespace PTTracker.Application.Common.Models;

public record ParsedEmail(
    string FromEmail,
    string? FromName,
    string Subject,
    DateTime SentAt,
    string? MessageId,
    string? InReplyTo,
    IReadOnlyList<string> References,
    string Body,
    string? BodyHtml,
    IReadOnlyList<ParsedAttachment> Attachments);

public record ParsedAttachment(
    string FileName,
    string ContentType,
    byte[] Content);
