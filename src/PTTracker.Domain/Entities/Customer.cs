using PTTracker.Domain.Common;

namespace PTTracker.Domain.Entities;

public class Customer : Entity
{
    private readonly List<Ticket> _tickets = new();

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Company { get; private set; }
    public string? Phone { get; private set; }

    public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    private Customer() { }

    public static Customer Create(
        string firstName,
        string lastName,
        string email,
        string? company = null,
        string? phone = null)
    {
        return new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Company = company,
            Phone = phone,
        };
    }
}
