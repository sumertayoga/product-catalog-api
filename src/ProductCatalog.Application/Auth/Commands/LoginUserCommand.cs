using ProductCatalog.Application.Common.Abstractions;
using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Exceptions;

namespace ProductCatalog.Application.Auth.Commands;

public sealed record LoginUserCommand(LoginRequest Request);

internal sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokenService) : ICommandHandler<LoginUserCommand, AuthResponse>
{
    public async Task<AuthResponse> HandleAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var email = EmailNormalizer.Normalize(command.Request.Email);
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        // Same message for unknown email and wrong password, so accounts can't be enumerated.
        var check = user is null
            ? PasswordCheck.Failed
            : passwordHasher.Verify(user.PasswordHash, command.Request.Password);
        if (user is null || check == PasswordCheck.Failed)
            throw new UnauthorizedException("Invalid email or password.");

        if (check == PasswordCheck.SuccessRehashNeeded)
        {
            user.ChangePasswordHash(passwordHasher.Hash(command.Request.Password));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return tokenService.CreateToken(user);
    }
}
