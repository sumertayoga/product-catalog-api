using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Auth;
using ProductCatalog.Application.Auth.Commands;
using ProductCatalog.Application.Common.Cqrs;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        [FromServices] ICommandHandler<RegisterUserCommand, AuthResponse> handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.HandleAsync(new RegisterUserCommand(request), cancellationToken));

    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginRequest request,
        [FromServices] ICommandHandler<LoginUserCommand, AuthResponse> handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.HandleAsync(new LoginUserCommand(request), cancellationToken));
}
