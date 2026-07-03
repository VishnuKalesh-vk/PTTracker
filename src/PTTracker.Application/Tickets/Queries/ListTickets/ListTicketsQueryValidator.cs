using FluentValidation;
using PTTracker.Domain.Enums;

namespace PTTracker.Application.Tickets.Queries.ListTickets;

public class ListTicketsQueryValidator : AbstractValidator<ListTicketsQuery>
{
    private static readonly string[] AllowedSortFields =
    {
        "createdat", "updatedat", "ticketnumber", "priority", "status", "subject",
    };

    public ListTicketsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x.SortBy)
            .Must(sortBy => AllowedSortFields.Contains(sortBy.ToLowerInvariant()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");

        RuleFor(x => x.Status)
            .IsEnumName(typeof(TicketStatus), caseSensitive: false)
            .When(x => !string.IsNullOrWhiteSpace(x.Status))
            .WithMessage("Status must be one of: New, Open, Pending, OnHold, Resolved, Closed.");

        RuleFor(x => x.Priority)
            .IsEnumName(typeof(TicketPriority), caseSensitive: false)
            .When(x => !string.IsNullOrWhiteSpace(x.Priority))
            .WithMessage("Priority must be one of: Low, Normal, High, Urgent.");
    }
}
