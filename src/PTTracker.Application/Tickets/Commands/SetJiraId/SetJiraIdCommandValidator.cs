using FluentValidation;

namespace PTTracker.Application.Tickets.Commands.SetJiraId;

public class SetJiraIdCommandValidator : AbstractValidator<SetJiraIdCommand>
{
    public SetJiraIdCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        // Allow clearing (null/empty). When provided, require a JIRA-style key e.g. DART-123.
        RuleFor(x => x.JiraId)
            .Matches(@"^[A-Za-z][A-Za-z0-9]*-\d+$")
            .When(x => !string.IsNullOrWhiteSpace(x.JiraId))
            .WithMessage("JIRA ID must look like 'PROJECT-123'.");
    }
}
