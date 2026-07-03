using PTTracker.Application.Common.Models;

namespace PTTracker.Application.Common.Interfaces;

public interface IEmailParser
{
    /// <summary>Parses a raw RFC822 (.eml) message. Throws <see cref="FormatException"/> on invalid input.</summary>
    ParsedEmail Parse(Stream emlStream);
}
