using FluentValidation;
using PTTracker.Domain.Enums;

namespace PTTracker.Application.Tickets.Commands.CreateManualTicket;

public class CreateManualTicketCommandValidator : AbstractValidator<CreateManualTicketCommand>
{
    public CreateManualTicketCommandValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Priority)
            .NotEmpty()
            .IsEnumName(typeof(TicketPriority), caseSensitive: false)
            .WithMessage("Priority must be one of: Low, Normal, High, Urgent.");
    }
}
