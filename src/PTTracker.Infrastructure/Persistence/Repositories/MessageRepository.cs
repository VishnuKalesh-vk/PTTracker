using Microsoft.EntityFrameworkCore;
using PTTracker.Application.Common.Interfaces;

namespace PTTracker.Infrastructure.Persistence.Repositories;

public class MessageRepository(AppDbContext dbContext) : IMessageRepository
{
    public async Task<Guid?> FindTicketIdByMessageAsync(
        string? messageIdHeader,
        string contentHash,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Messages.AsNoTracking();

        if (!string.IsNullOrEmpty(messageIdHeader))
        {
            var byHeader = await query
                .Where(m => m.MessageIdHeader == messageIdHeader)
                .Select(m => (Guid?)m.TicketId)
                .FirstOrDefaultAsync(cancellationToken);

            if (byHeader is not null)
                return byHeader;
        }

        return await query
            .Where(m => m.ContentHash == contentHash)
            .Select(m => (Guid?)m.TicketId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid?> FindTicketIdByHeaderIdsAsync(
        IReadOnlyCollection<string> headerIds,
        CancellationToken cancellationToken = default)
    {
        if (headerIds.Count == 0)
            return null;

        return await dbContext.Messages
            .AsNoTracking()
            .Where(m => m.MessageIdHeader != null && headerIds.Contains(m.MessageIdHeader))
            .OrderBy(m => m.SentAt)
            .Select(m => (Guid?)m.TicketId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
