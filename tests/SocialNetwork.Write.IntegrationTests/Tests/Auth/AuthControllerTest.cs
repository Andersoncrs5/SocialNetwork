using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Auth;

public class AuthControllerTest : BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;

    public AuthControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }
    
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
        http.DetailsError.Should().BeNullOrWhiteSpace();
        http.Success.Should().BeTrue();
    }
    
    [Fact]
    public async Task ShouldLoginUser_Success()
    {
        UserTestResult result = await _helper.CreateNewUser();

        LoginUserDto dto = new LoginUserDto()
        {
            Email = result.Dto.Email,
            Password = result.Dto.PasswordHash,
        };

        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/Auth/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<ResponseTokens>? http = await response.Content.ReadFromJsonAsync<ResponseHttp<ResponseTokens>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Data.Token.Should().NotBeNullOrEmpty();
        http.Data.RefreshToken.Should().NotBeNullOrEmpty();
        http.DetailsError.Should().BeNullOrWhiteSpace();
        http.Success.Should().BeTrue();
    }
    
    [Fact]
    public async Task ShouldLoginUser_FailBecauseEmailWrong()
    {
        UserTestResult result = await _helper.CreateNewUser();

        LoginUserDto dto = new LoginUserDto()
        {
            Email = "user142543@example.com",
            Password = result.Dto.PasswordHash,
        };

        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/Auth/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        ResponseHttp<object>? http = await response.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        
        http.Data.Should().BeNull();
        http.DetailsError.Should().BeNullOrWhiteSpace();
        http.Success.Should().BeFalse();
    }
    
    [Fact]
    public async Task ShouldLoginUser_FailBecausePasswordWrong()
    {
        UserTestResult result = await _helper.CreateNewUser();

        LoginUserDto dto = new LoginUserDto()
        {
            Email = result.Dto.Email,
            Password = result.Dto.PasswordHash + "aa",
        };

        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/v1/Auth/login", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        ResponseHttp<object>? http = await response.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        
        http.Data.Should().BeNull();
        http.DetailsError.Should().BeNullOrWhiteSpace();
        http.Success.Should().BeFalse();
    }
    
    [Fact]
    public async Task Logout_Success()
    {
        UserTestResult result = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);
        
        HttpResponseMessage message = await Client.PostAsJsonAsync("/api/v1/Auth/logout", result);
        
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
    }
    
    [Fact]
    public async Task Logout_Fail_TokenRequired()
    {
        UserTestResult result = await _helper.CreateNewUser();
        
        HttpResponseMessage message = await Client.PostAsJsonAsync("/api/v1/Auth/logout", result);
        
        message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
}