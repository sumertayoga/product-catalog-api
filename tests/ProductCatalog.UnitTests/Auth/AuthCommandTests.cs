using NSubstitute;
using ProductCatalog.Application.Auth;
using ProductCatalog.Application.Auth.Commands;
using ProductCatalog.Application.Common.Abstractions;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.UnitTests.Auth;

public class AuthCommandTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _tokens = Substitute.For<IJwtTokenService>();

    [Fact]
    public async Task Register_StoresLowercaseEmail()
    {
        _hasher.Hash("Password123").Returns("hash");
        var handler = new RegisterUserCommandHandler(_users, _unitOfWork, _hasher, _tokens, TimeProvider.System);

        await handler.HandleAsync(
            new RegisterUserCommand(new RegisterRequest(" User@Example.com ", "Password123")),
            CancellationToken.None);

        _users.Received(1).Add(Arg.Is<User>(u => u.Email == "user@example.com" && u.PasswordHash == "hash"));
    }

    [Fact]
    public async Task Register_DuplicateEmail_ThrowsConflict()
    {
        _users.ExistsByEmailAsync("user@example.com", Arg.Any<CancellationToken>()).Returns(true);
        var handler = new RegisterUserCommandHandler(_users, _unitOfWork, _hasher, _tokens, TimeProvider.System);

        await Assert.ThrowsAsync<ConflictException>(() => handler.HandleAsync(
            new RegisterUserCommand(new RegisterRequest("user@example.com", "Password123")),
            CancellationToken.None));
    }

    [Fact]
    public async Task Login_UnknownEmailAndWrongPassword_GiveSameError()
    {
        var user = User.Create("user@example.com", "hash", DateTime.UtcNow);
        _users.GetByEmailAsync("user@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("hash", "wrong-password").Returns(PasswordCheck.Failed);
        var handler = new LoginUserCommandHandler(_users, _unitOfWork, _hasher, _tokens);

        var unknown = await Assert.ThrowsAsync<UnauthorizedException>(() => handler.HandleAsync(
            new LoginUserCommand(new LoginRequest("nobody@example.com", "wrong-password")),
            CancellationToken.None));
        var wrongPassword = await Assert.ThrowsAsync<UnauthorizedException>(() => handler.HandleAsync(
            new LoginUserCommand(new LoginRequest("user@example.com", "wrong-password")),
            CancellationToken.None));

        Assert.Equal(unknown.Message, wrongPassword.Message);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var user = User.Create("user@example.com", "hash", DateTime.UtcNow);
        var expected = new AuthResponse("token", DateTime.UtcNow.AddHours(1));
        _users.GetByEmailAsync("user@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("hash", "Password123").Returns(PasswordCheck.Success);
        _tokens.CreateToken(user).Returns(expected);
        var handler = new LoginUserCommandHandler(_users, _unitOfWork, _hasher, _tokens);

        var result = await handler.HandleAsync(
            new LoginUserCommand(new LoginRequest("user@example.com", "Password123")),
            CancellationToken.None);

        Assert.Same(expected, result);
    }
}
