using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests;

public class HelperTest(HttpClient client)
{
    public async Task<UserTestResult> LoginMaster()
    {
        LoginUserDto dto = new LoginUserDto()
        {
            Email = "usersystem@example.com",
            Password = "Aw53653%54Fe!",
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/Auth/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<ResponseTokens>? http = await response.Content.ReadFromJsonAsync<ResponseHttp<ResponseTokens>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        http.Data.User.Should().NotBeNull();
        
        http.Data.Token.Should().NotBeNullOrEmpty();
        http.Data.RefreshToken.Should().NotBeNullOrEmpty();
        http.DetailsError.Should().BeNullOrWhiteSpace();
        http.Success.Should().BeTrue();

        CreateUserDto dtoRe = new CreateUserDto()
        {
            Email = http.Data.User.Email!,
            Username = http.Data.User.Username!,
            FullName = http.Data.User.FullName,
            PasswordHash = dto.Password
        };
        
        return new UserTestResult()
        {
            Dto = dtoRe ,
            Tokens = http.Data,
            User = http.Data.User,
        };
    }
    
    public async Task<UserTestResult> CreateNewUser()
    {
        long num = Random.Shared.NextInt64(1, 10000000000000);
        
        CreateUserDto dto = new CreateUserDto()
        {
            Email = $"user{num}@gmail.com",
            PasswordHash = "test6rA553e463$#%$%",
            Username = "pochita" + num,
            FullName = "pochita the chainsaw demon"
        };
        
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/Auth/register", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<ResponseTokens>? http = await response.Content.ReadFromJsonAsync<ResponseHttp<ResponseTokens>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        http.Data.User.Should().NotBeNull();
        
        http.Data.Token.Should().NotBeNullOrEmpty();
        http.Data.RefreshToken.Should().NotBeNullOrEmpty();
        http.DetailsError.Should().BeNullOrWhiteSpace();
        http.Success.Should().BeTrue();

        return new UserTestResult()
        {
            Dto = dto,
            Tokens = http.Data,
            User = http.Data.User,
        };

    }

    public string GenerateChars()
    {
        return string.Create(25, (object?)null, (buffer, _) =>
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                int asciiCode = Random.Shared.Next(65, 91); 
                buffer[i] = (char)asciiCode;
            }
        });
    }
    
}