using Ganss.Xss;

namespace PTTracker.Infrastructure.Html;

public class HtmlSanitizerAdapter : Application.Common.Interfaces.IHtmlSanitizer
{
    // HtmlSanitizer.Sanitize is thread-safe; safe to share a single instance.
    private readonly HtmlSanitizer _sanitizer = new();

    public string Sanitize(string html) => _sanitizer.Sanitize(html);
}
