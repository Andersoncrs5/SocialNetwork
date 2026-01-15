using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    ResponseTokens CreateTokens(UserModel user, IList<string> userRoles);
}