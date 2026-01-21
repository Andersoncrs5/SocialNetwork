using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Renci.SshNet.Security;
using SocialNetwork.Contracts.configs.jwt;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.InfoApp;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Services.Providers;

public class TokenService(
    IOptions<JwtOptions> jwtOptions,
    IOptions<InfoAppOptions> infoAppOptions
    ) : ITokenService
{
    private readonly JwtOptions _options = jwtOptions.Value;
    private readonly InfoAppOptions _appOptions = infoAppOptions.Value;
    private readonly string _alg = SecurityAlgorithms.HmacSha256;

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, _alg);
        
        var token = new JwtSecurityToken(
            issuer: _options.ValidIssuer,
            audience: _options.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.TokenValidityInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public ResponseTokens CreateTokens(UserModel user, IList<string> userRoles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sid, user.Id),
            new Claim(JwtRegisteredClaimNames.Iss, _appOptions.Domain),
            new Claim(JwtRegisteredClaimNames.Aud, _options.ValidAudience),
            new Claim("country", user.Country ?? string.Empty),
            new Claim("language", user.Language ?? string.Empty),
            new Claim("lockoutEnd", user.LockoutEnd.ToString() ?? string.Empty),
        };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var accessToken = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_options.RefreshTokenValidityInMinutes);

        return new ResponseTokens
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiredAt = DateTime.UtcNow.AddMinutes(_options.TokenValidityInMinutes),
            ExpiredAtRefreshToken = user.RefreshTokenExpiryTime ?? DateTime.UtcNow
        };
    }
}