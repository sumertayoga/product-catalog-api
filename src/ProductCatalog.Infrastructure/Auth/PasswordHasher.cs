using Microsoft.AspNetCore.Identity;
using ProductCatalog.Application.Common.Abstractions;

namespace ProductCatalog.Infrastructure.Auth;

internal sealed class PasswordHasher : IPasswordHasher
{
    // ASP.NET Core Identity's PBKDF2 hasher.
    private static readonly object User = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(User, password);

    public PasswordCheck Verify(
        string passwordHash,
        string password) =>
        _hasher.VerifyHashedPassword(User, passwordHash, password) switch
        {
            PasswordVerificationResult.Success => PasswordCheck.Success,
            PasswordVerificationResult.SuccessRehashNeeded => PasswordCheck.SuccessRehashNeeded,
            _ => PasswordCheck.Failed
        };
}
