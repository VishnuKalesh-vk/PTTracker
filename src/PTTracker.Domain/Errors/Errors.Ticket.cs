using ErrorOr;

namespace PTTracker.Domain.Errors;

public static partial class Errors
{
    public static class Ticket
    {
        public static Error NotFound => Error.NotFound(
            code: "Ticket.NotFound",
            description: "Ticket not found.");
    }
}
