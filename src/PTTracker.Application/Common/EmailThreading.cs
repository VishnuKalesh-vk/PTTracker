using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PTTracker.Application.Common;

/// <summary>Helpers for grouping emails into ticket threads and computing idempotency hashes.</summary>
public static partial class EmailThreading
{
    [GeneratedRegex(@"^(re|fwd|fw|aw|sv)\s*:\s*", RegexOptions.IgnoreCase)]
    private static partial Regex ReplyPrefixRegex();

    public static string NormalizeSubject(string? subject)
    {
        var current = (subject ?? string.Empty).Trim();
        string previous;
        do
        {
            previous = current;
            current = ReplyPrefixRegex().Replace(current, string.Empty).Trim();
        }
        while (previous != current);

        return current;
    }

    /// <summary>Stable thread key derived from the normalized subject (used for subject-fallback grouping).</summary>
    public static string SubjectThreadKey(string? subject)
    {
        var normalized = NormalizeSubject(subject).ToLowerInvariant();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        return "subj:" + Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static string ContentHash(byte[] content) =>
        Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
}
