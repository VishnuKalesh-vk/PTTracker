using ErrorOr;
using MediatR;
using PTTracker.Application.Authentication.Common;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Domain.Entities;
using PTTracker.Domain.Errors;

namespace PTTracker.Application.Authentication.Queries.Login;

public class LoginQueryHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthTokenService authTokenService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(
        LoginQuery query,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(query.Email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(query.Password, user.PasswordHash))
            return Errors.Authentication.InvalidCredentials;

        var tokens = authTokenService.Issue(user);
        var refreshToken = RefreshToken.Create(
            user.Id,
            authTokenService.HashRefreshToken(tokens.RefreshToken),
            tokens.RefreshTokenExpiresAt);
        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(user, tokens);
    }
}
