using ProductCatalog.Application.Common.Abstractions;
using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Auth.Commands;

public sealed record RegisterUserCommand(RegisterRequest Request);

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokenService,
    TimeProvider timeProvider) : ICommandHandler<RegisterUserCommand, AuthResponse>
{
    public async Task<AuthResponse> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var email = EmailNormalizer.Normalize(command.Request.Email);

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new ConflictException("Email is already registered.");

        var user = User.Create(
            email,
            passwordHasher.Hash(command.Request.Password),
            timeProvider.GetUtcNow().UtcDateTime);

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return tokenService.CreateToken(user);
    }
}
