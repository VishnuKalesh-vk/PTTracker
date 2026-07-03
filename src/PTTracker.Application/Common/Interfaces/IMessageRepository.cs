namespace PTTracker.Application.Common.Interfaces;

public interface IMessageRepository
{
    /// <summary>Returns the ticket id of an already-ingested message (by header or content hash), for dedup.</summary>
    Task<Guid?> FindTicketIdByMessageAsync(string? messageIdHeader, string contentHash, CancellationToken cancellationToken = default);

    /// <summary>Returns the ticket id owning any message whose Message-ID is in <paramref name="headerIds"/> (reply-chain match).</summary>
    Task<Guid?> FindTicketIdByHeaderIdsAsync(IReadOnlyCollection<string> headerIds, CancellationToken cancellationToken = default);
}
