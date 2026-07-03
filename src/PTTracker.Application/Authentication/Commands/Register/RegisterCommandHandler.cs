using ErrorOr;
using MediatR;
using PTTracker.Application.Authentication.Common;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Domain.Entities;
using PTTracker.Domain.Errors;
using DomainRefreshToken = PTTracker.Domain.Entities.RefreshToken;

namespace PTTracker.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthTokenService authTokenService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        if (await userRepository.GetByEmailAsync(command.Email, cancellationToken) is not null)
            return Errors.User.DuplicateEmail;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        var user = User.Create(command.FirstName, command.LastName, command.Email, passwordHash);
        await userRepository.AddAsync(user, cancellationToken);

        var tokens = authTokenService.Issue(user);
        var refreshToken = DomainRefreshToken.Create(
            user.Id,
            authTokenService.HashRefreshToken(tokens.RefreshToken),
            tokens.RefreshTokenExpiresAt);
        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(user, tokens);
    }
}
