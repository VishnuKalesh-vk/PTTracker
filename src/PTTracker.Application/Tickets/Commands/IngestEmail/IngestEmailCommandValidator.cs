using FluentValidation;

namespace PTTracker.Application.Tickets.Commands.IngestEmail;

public class IngestEmailCommandValidator : AbstractValidator<IngestEmailCommand>
{
    public IngestEmailCommandValidator()
    {
        RuleFor(x => x.EmlContent).NotEmpty().WithMessage("The .eml file is empty.");
        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(name => name.EndsWith(".eml", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only .eml files are accepted.");
    }
}
