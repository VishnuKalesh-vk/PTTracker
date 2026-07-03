using ErrorOr;
using MediatR;
using PTTracker.Application.Authentication.Common;

namespace PTTracker.Application.Authentication.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;
