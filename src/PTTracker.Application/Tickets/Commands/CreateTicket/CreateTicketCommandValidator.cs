using FluentValidation;
using PTTracker.Domain.Enums;

namespace PTTracker.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Priority)
            .NotEmpty()
            .IsEnumName(typeof(TicketPriority), caseSensitive: false)
            .WithMessage("Priority must be one of: Low, Normal, High, Urgent.");
    }
}
