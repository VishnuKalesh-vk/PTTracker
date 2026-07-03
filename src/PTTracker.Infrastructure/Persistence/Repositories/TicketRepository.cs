using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Common.Models;
using PTTracker.Domain.Entities;

namespace PTTracker.Infrastructure.Persistence.Repositories;

public class TicketRepository(AppDbContext dbContext) : ITicketRepository
{
    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
        => await dbContext.Tickets.AddAsync(ticket, cancellationToken);

    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Tickets
            .Include(t => t.Customer)
            .Include(t => t.AssignedUser)
            .Include(t => t.TicketTags)
                .ThenInclude(tt => tt.Tag)
            .Include(t => t.Messages)
                .ThenInclude(m => m.Attachments)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public void Remove(Ticket ticket) => dbContext.Tickets.Remove(ticket);

    public async Task<Ticket?> FindByThreadAsync(
        Guid customerId,
        string threadKey,
        DateTime since,
        CancellationToken cancellationToken = default)
        => await dbContext.Tickets
            .Where(t => t.CustomerId == customerId && t.ThreadKey == threadKey && t.CreatedAt >= since)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedList<Ticket>> GetPagedAsync(
        TicketFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Tickets
            .AsNoTracking()
            .Include(t => t.Customer)
            .Include(t => t.AssignedUser)
            .AsQueryable();

        if (filter.Status is not null)
            query = query.Where(t => t.Status == filter.Status);

        if (filter.Priority is not null)
            query = query.Where(t => t.Priority == filter.Priority);

        if (filter.CustomerId is not null)
            query = query.Where(t => t.CustomerId == filter.CustomerId);

        if (filter.AssignedUserId is not null)
            query = query.Where(t => t.AssignedUserId == filter.AssignedUserId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var pattern = $"%{filter.Search.Trim()}%";
            query = query.Where(t => EF.Functions.ILike(t.Subject, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySort(query, filter.SortBy, filter.SortDescending);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<Ticket>(items, totalCount, filter.Page, filter.PageSize);
    }

    private static IQueryable<Ticket> ApplySort(IQueryable<Ticket> query, string sortBy, bool descending)
    {
        Expression<Func<Ticket, object>> keySelector = sortBy.ToLowerInvariant() switch
        {
            "subject" => t => t.Subject,
            "status" => t => t.Status,
            "priority" => t => t.Priority,
            "ticketnumber" => t => t.TicketNumber,
            "updatedat" => t => t.UpdatedAt,
            _ => t => t.CreatedAt,
        };

        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
