using ErrorOr;
using MediatR;
using PTTracker.Application.Authentication.Common;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Domain.Errors;
using DomainRefreshToken = PTTracker.Domain.Entities.RefreshToken;

namespace PTTracker.Application.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IAuthTokenService authTokenService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RefreshTokenCommand, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var hash = authTokenService.HashRefreshToken(command.RefreshToken);
        var stored = await refreshTokenRepository.GetByHashAsync(hash, cancellationToken);

        if (stored is null || !stored.IsActive)
            return Errors.Authentication.InvalidRefreshToken;

        // Rotate: revoke the presented token, issue a fresh pair.
        stored.Revoke();

        var tokens = authTokenService.Issue(stored.User);
        var replacement = DomainRefreshToken.Create(
            stored.UserId,
            authTokenService.HashRefreshToken(tokens.RefreshToken),
            tokens.RefreshTokenExpiresAt);
        await refreshTokenRepository.AddAsync(replacement, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(stored.User, tokens);
    }
}
