using PTTracker.Application.Common.Models;
using PTTracker.Domain.Entities;

namespace PTTracker.Application.Common.Interfaces;

public interface IAuthTokenService
{
    /// <summary>Issues a fresh access token (JWT) and a new opaque refresh token for the user.</summary>
    AuthTokens Issue(User user);

    /// <summary>Hashes a raw refresh token for storage/lookup (never store the raw value).</summary>
    string HashRefreshToken(string rawToken);
}
