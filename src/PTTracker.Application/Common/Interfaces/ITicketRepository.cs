using PTTracker.Application.Common.Models;
using PTTracker.Domain.Entities;

namespace PTTracker.Application.Common.Interfaces;

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);

    /// <summary>Loads a ticket with Customer, AssignedUser, and Tags eagerly included.</summary>
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Remove(Ticket ticket);

    /// <summary>Most recent tracked ticket for a customer matching <paramref name="threadKey"/> since the cutoff (subject-fallback grouping).</summary>
    Task<Ticket?> FindByThreadAsync(Guid customerId, string threadKey, DateTime since, CancellationToken cancellationToken = default);

    Task<PagedList<Ticket>> GetPagedAsync(TicketFilter filter, CancellationToken cancellationToken = default);
}
