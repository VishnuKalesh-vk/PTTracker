using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Application.Common.Models;
using PTTracker.Domain.Entities;

namespace PTTracker.Infrastructure.Authentication;

public class AuthTokenService(IOptions<JwtSettings> jwtOptions) : IAuthTokenService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public AuthTokens Issue(User user)
    {
        var accessExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        var accessToken = GenerateAccessToken(user, accessExpiresAt);

        var refreshToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

        return new AuthTokens(accessToken, accessExpiresAt, refreshToken, refreshExpiresAt);
    }

    public string HashRefreshToken(string rawToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken))).ToLowerInvariant();

    private string GenerateAccessToken(User user, DateTime expiresAt)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: expiresAt,
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}
