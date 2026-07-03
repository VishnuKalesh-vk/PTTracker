namespace PTTracker.Contracts.Tickets;

public record MessageResponse(
    Guid Id,
    string SenderType,
    string Body,
    string? BodyHtml,
    DateTime SentAt,
    IReadOnlyList<AttachmentResponse> Attachments);

public record AttachmentResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes);
