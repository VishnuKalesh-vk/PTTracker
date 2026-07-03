using PTTracker.Domain.Common;
using PTTracker.Domain.Enums;

namespace PTTracker.Domain.Entities;

public class User : Entity
{
    private readonly List<Ticket> _assignedTickets = new();
    private readonly List<Note> _notes = new();

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }

    public IReadOnlyCollection<Ticket> AssignedTickets => _assignedTickets.AsReadOnly();
    public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();

    private User() { }

    public static User Create(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        UserRole role = UserRole.Agent)
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
        };
    }
}
