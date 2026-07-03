using PTTracker.Domain.Common;
using PTTracker.Domain.Enums;

namespace PTTracker.Domain.Entities;

public class Ticket : Entity
{
    private readonly List<Message> _messages = new();
    private readonly List<Note> _notes = new();
    private readonly List<StatusHistory> _statusHistory = new();
    private readonly List<TicketTag> _ticketTags = new();

    // Human-readable sequential identifier, populated by a DB sequence.
    public long TicketNumber { get; private set; }
    public string Subject { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    // Sanitized rich-text (HTML) description; null when the description is plain text only.
    public string? DescriptionHtml { get; private set; }
    public TicketStatus Status { get; private set; }
    public TicketPriority Priority { get; private set; }

    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    public Guid? AssignedUserId { get; private set; }
    public User? AssignedUser { get; private set; }

    public DateTime? ResolvedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    // Threading + provenance for ingested email. ThreadKey groups messages that
    // belong together (References-root id or normalized-subject hash).
    public string? ThreadKey { get; private set; }
    public SourceProvider Source { get; private set; }

    // External JIRA issue key (e.g. "DART-123"); null until assigned.
    public string? JiraId { get; private set; }

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();
    public IReadOnlyCollection<StatusHistory> StatusHistory => _statusHistory.AsReadOnly();
    public IReadOnlyCollection<TicketTag> TicketTags => _ticketTags.AsReadOnly();

    private Ticket() { }

    public static Ticket Create(
        Guid customerId,
        string subject,
        string description,
        TicketPriority priority = TicketPriority.Normal,
        Guid? assignedUserId = null,
        SourceProvider source = SourceProvider.Manual,
        string? threadKey = null,
        string? descriptionHtml = null)
    {
        var ticket = new Ticket
        {
            CustomerId = customerId,
            Subject = subject,
            Description = description,
            DescriptionHtml = descriptionHtml,
            Priority = priority,
            Status = TicketStatus.New,
            AssignedUserId = assignedUserId,
            Source = source,
            ThreadKey = threadKey,
        };

        ticket._statusHistory.Add(
            Entities.StatusHistory.Create(ticket.Id, null, TicketStatus.New, assignedUserId, "Ticket created"));

        return ticket;
    }

    public void AddMessage(Message message)
    {
        _messages.Add(message);
        Touch();
    }

    public void ChangeStatus(TicketStatus newStatus, Guid? changedByUserId, string? reason = null)
    {
        if (newStatus == Status)
            return;

        var previous = Status;
        Status = newStatus;
        Touch();

        ResolvedAt = newStatus == TicketStatus.Resolved ? DateTime.UtcNow : ResolvedAt;
        ClosedAt = newStatus == TicketStatus.Closed ? DateTime.UtcNow : ClosedAt;

        _statusHistory.Add(
            Entities.StatusHistory.Create(Id, previous, newStatus, changedByUserId, reason));
    }

    public void UpdateDetails(string subject, string description, string? descriptionHtml)
    {
        Subject = subject;
        Description = description;
        DescriptionHtml = descriptionHtml;
        Touch();
    }

    public void Assign(Guid? userId)
    {
        AssignedUserId = userId;
        Touch();
    }

    public void SetJiraId(string? jiraId)
    {
        JiraId = string.IsNullOrWhiteSpace(jiraId) ? null : jiraId.Trim().ToUpperInvariant();
        Touch();
    }

    public void ChangePriority(TicketPriority priority)
    {
        Priority = priority;
        Touch();
    }
}
