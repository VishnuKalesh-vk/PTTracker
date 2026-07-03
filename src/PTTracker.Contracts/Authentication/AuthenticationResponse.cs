namespace PTTracker.Contracts.Authentication;

public record AuthenticationResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken);
