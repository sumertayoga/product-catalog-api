using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Auth;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken);

    void Add(User user);
}
