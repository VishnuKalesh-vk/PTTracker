using ErrorOr;
using MediatR;
using PTTracker.Application.Authentication.Common;

namespace PTTracker.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ErrorOr<AuthenticationResult>>;
