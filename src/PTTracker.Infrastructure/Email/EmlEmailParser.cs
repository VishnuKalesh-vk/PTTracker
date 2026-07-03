using System.Text.RegularExpressions;
using MimeKit;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Common.Models;

namespace PTTracker.Infrastructure.Email;

public partial class EmlEmailParser(IHtmlSanitizer sanitizer) : IEmailParser
{
    public ParsedEmail Parse(Stream emlStream)
    {
        var message = MimeMessage.Load(emlStream);

        var from = message.From.Mailboxes.FirstOrDefault()
                   ?? throw new FormatException("Email has no From address.");

        var body = !string.IsNullOrEmpty(message.TextBody)
            ? message.TextBody
            : HtmlToText(message.HtmlBody);

        var bodyHtml = string.IsNullOrEmpty(message.HtmlBody)
            ? null
            : sanitizer.Sanitize(message.HtmlBody);

        var sentAt = message.Date == DateTimeOffset.MinValue
            ? DateTime.UtcNow
            : message.Date.UtcDateTime;

        var references = message.References?.ToList() ?? new List<string>();

        var attachments = new List<ParsedAttachment>();
        foreach (var entity in message.Attachments)
        {
            if (entity is not MimePart { Content: not null } part)
                continue;

            using var memory = new MemoryStream();
            part.Content.DecodeTo(memory);

            attachments.Add(new ParsedAttachment(
                part.FileName ?? "attachment",
                part.ContentType.MimeType,
                memory.ToArray()));
        }

        return new ParsedEmail(
            from.Address,
            string.IsNullOrWhiteSpace(from.Name) ? null : from.Name,
            message.Subject ?? string.Empty,
            sentAt,
            string.IsNullOrWhiteSpace(message.MessageId) ? null : message.MessageId,
            string.IsNullOrWhiteSpace(message.InReplyTo) ? null : message.InReplyTo,
            references,
            body,
            bodyHtml,
            attachments);
    }

    private static string HtmlToText(string? html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        var withoutTags = TagRegex().Replace(html, " ");
        return WhitespaceRegex().Replace(System.Net.WebUtility.HtmlDecode(withoutTags), " ").Trim();
    }

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex TagRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
