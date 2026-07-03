using PTTracker.Application.Common.Models;
using PTTracker.Domain.Entities;

namespace PTTracker.Application.Authentication.Common;

public record AuthenticationResult(User User, AuthTokens Tokens);
