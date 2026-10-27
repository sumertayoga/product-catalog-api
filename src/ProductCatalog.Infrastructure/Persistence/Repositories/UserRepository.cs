using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Auth;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

    public Task<bool> ExistsByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

    public void Add(User user) => dbContext.Users.Add(user);
}
