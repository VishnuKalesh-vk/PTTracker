namespace PTTracker.Application.Common.Interfaces;

public interface IHtmlSanitizer
{
    /// <summary>Removes scripts, event handlers, and other unsafe markup from untrusted HTML.</summary>
    string Sanitize(string html);
}
