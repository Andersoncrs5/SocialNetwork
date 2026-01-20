using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.User;

namespace SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;

public class UserTestResult
{
    public required CreateUserDto Dto { get; set; }
    public UserDto User { get; set; }
    public required ResponseTokens Tokens { get; set; }
}