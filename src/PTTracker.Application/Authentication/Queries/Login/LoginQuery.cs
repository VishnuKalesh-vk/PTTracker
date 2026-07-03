using ErrorOr;
using MediatR;
using PTTracker.Application.Authentication.Common;

namespace PTTracker.Application.Authentication.Queries.Login;

public record LoginQuery(string Email, string Password) : IRequest<ErrorOr<AuthenticationResult>>;
