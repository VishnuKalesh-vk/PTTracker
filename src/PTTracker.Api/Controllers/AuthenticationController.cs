using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTTracker.Application.Authentication.Commands.Register;
using PTTracker.Application.Authentication.Commands.RefreshToken;
using PTTracker.Application.Authentication.Common;
using PTTracker.Application.Authentication.Queries.Login;
using PTTracker.Contracts.Authentication;

namespace PTTracker.Api.Controllers;

[Route("api/auth")]
[AllowAnonymous]
public class AuthenticationController(ISender mediator) : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        var result = await mediator.Send(command);

        return result.Match(ToResponse, errors => Problem(errors));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = new LoginQuery(request.Email, request.Password);

        var result = await mediator.Send(query);

        return result.Match(ToResponse, errors => Problem(errors));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);

        var result = await mediator.Send(command);

        return result.Match(ToResponse, errors => Problem(errors));
    }

    private IActionResult ToResponse(AuthenticationResult result) =>
        Ok(new AuthenticationResponse(
            result.User.Id,
            result.User.FirstName,
            result.User.LastName,
            result.User.Email,
            result.Tokens.AccessToken,
            result.Tokens.AccessTokenExpiresAt,
            result.Tokens.RefreshToken));
}
