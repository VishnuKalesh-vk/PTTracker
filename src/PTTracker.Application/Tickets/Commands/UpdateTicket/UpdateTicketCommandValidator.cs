using FluentValidation;
using PTTracker.Domain.Enums;

namespace PTTracker.Application.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Status)
            .NotEmpty()
            .IsEnumName(typeof(TicketStatus), caseSensitive: false)
            .WithMessage("Status must be one of: New, Open, Pending, OnHold, Resolved, Closed.");
        RuleFor(x => x.Priority)
            .NotEmpty()
            .IsEnumName(typeof(TicketPriority), caseSensitive: false)
            .WithMessage("Priority must be one of: Low, Normal, High, Urgent.");
    }
}
