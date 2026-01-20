using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.IntegrationTests.Config;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Auth;

public class AuthControllerTest(WriteApiFactory factory, ITestOutputHelper output): BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateNewUser_Success()
    {
        long num = Random.Shared.NextInt64(1, 10000000000000);
        
        CreateUserDto dto = new CreateUserDto()
        {
            Email = $"user{num}@gmail.com",
            PasswordHash = "test6rA553e463$#%$%",
            Username = "pochita" + num,
            FullName = "pochita the chainsaw demon"
        };
        
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/Auth/register", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<ResponseTokens>? http = await response.Content.ReadFromJsonAsync<ResponseHttp<ResponseTokens>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Data.Token.Should().NotBeNullOrEmpty();
        http.Data.RefreshToken.Should().NotBeNullOrEmpty();
        http.ErrorCode.Should().Be(0);
        http.DetailsError.Should().BeNullOrWhiteSpace();
        http.Success.Should().BeTrue();

    }
}