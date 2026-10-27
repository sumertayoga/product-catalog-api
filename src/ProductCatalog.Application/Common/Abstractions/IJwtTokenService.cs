using ProductCatalog.Application.Auth;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Common.Abstractions;

public interface IJwtTokenService
{
    AuthResponse CreateToken(User user);
}
