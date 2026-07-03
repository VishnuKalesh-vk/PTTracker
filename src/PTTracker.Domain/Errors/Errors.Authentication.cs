using ErrorOr;

namespace PTTracker.Domain.Errors;

public static partial class Errors
{
    public static class Authentication
    {
        public static Error InvalidCredentials => Error.Validation(
            code: "Auth.InvalidCredentials",
            description: "Invalid credentials.");

        public static Error InvalidRefreshToken => Error.Unauthorized(
            code: "Auth.InvalidRefreshToken",
            description: "The refresh token is invalid, expired, or revoked.");
    }
}
