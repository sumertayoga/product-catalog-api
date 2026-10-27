using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ProductCatalog.Application.Auth;
using ProductCatalog.Application.Common.Abstractions;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Auth;

internal sealed class JwtTokenService(
    IOptions<JwtOptions> options,
    TimeProvider timeProvider) : IJwtTokenService
{
    private readonly JsonWebTokenHandler _tokenHandler = new();

    public AuthResponse CreateToken(User user)
    {
        var jwt = options.Value;
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = now.AddMinutes(jwt.ExpiryMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        return new AuthResponse(_tokenHandler.CreateToken(descriptor), expiresAt);
    }
}
