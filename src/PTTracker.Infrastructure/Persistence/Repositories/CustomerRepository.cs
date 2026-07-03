using Microsoft.EntityFrameworkCore;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Domain.Entities;

namespace PTTracker.Infrastructure.Persistence.Repositories;

public class CustomerRepository(AppDbContext dbContext) : ICustomerRepository
{
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Customers.AnyAsync(c => c.Id == id, cancellationToken);

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
        => await dbContext.Customers.AddAsync(customer, cancellationToken);
}
