using DijitalEvrakTakip.Application.Options;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _settings;

    public JwtProvider(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string Token, DateTime Expiration) CreateToken(UserLoginDto user)
    {
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("name", user.Name ?? string.Empty),
            new Claim("surname", user.Surname ?? string.Empty),
            new Claim("departmentId", user.DepartmentId ?? string.Empty),
            new Claim("departmentName", user.DepartmentName ?? string.Empty),
            new Claim("departmentShortName", user.DepartmentShortName ?? string.Empty)
        };

        foreach (string role in user.Roles ?? Array.Empty<string>())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        DateTime expiration = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_settings.SecretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiration,
            signingCredentials: credentials);

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expiration);
    }
}
