using ProductCatalog.Domain.Exceptions;

namespace ProductCatalog.Domain.Entities;

public sealed class User
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private User()
    {
    }

    public static User Create(string normalizedEmail,
        string passwordHash,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(normalizedEmail))
            throw new DomainException("Email is required.");

        var user = new User
        {
            Email = normalizedEmail,
            CreatedAt = createdAtUtc
        };
        user.ChangePasswordHash(passwordHash);
        return user;
    }

    public void ChangePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        PasswordHash = passwordHash;
    }
}
