using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.User;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Auth;

public class UserControllerTest : BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/user";

    public UserControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task DeleteUser_Success()
    {
        UserTestResult result = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync(_url);
        _output.WriteLine(await message.Content.ReadAsStringAsync());
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Data.Should().BeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();
    }

    [Fact]
    public async Task UpdateUser_Success_AllFields()
    {
        UserTestResult result = await _helper.CreateNewUser();

        UpdateUserDto dto = new UpdateUserDto()
        {
            Bio = "AnyBio",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Country = "USA",
            CoverImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTQ0SFRurRwxC7u88oa0JQRTFzo0wn7XTud4Q&s",
            FullName = "pochita the chainsaw demon",
            ImageProfileUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTQ0SFRurRwxC7u88oa0JQRTFzo0wn7XTud4Q&s",
            IsPrivate = false,
            Language = "EN",
            PasswordHash = "AnbbgygyyGTD4866756%%$%f",
            Username = $"{result.User.Username}1"
       };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        HttpResponseMessage message = await Client.PatchAsJsonAsync(_url, dto);
        _output.WriteLine(await message.Content.ReadAsStringAsync());
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<UserDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<UserDto>>();
        
        http.Should().NotBeNull();
        http.DetailsError.Should().BeNull();
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().NotBeNull();
        http.Data.Id.Should().Be(result.User.Id);
        http.Data.Bio.Should().NotBe(result.User.Bio);
        http.Data.BirthDate.Should().NotBe(result.User.BirthDate);
        http.Data.Email.Should().Be(result.User.Email);
        http.Data.Country.Should().NotBe(result.User.Country);
        http.Data.CoverImageUrl.Should().NotBe(result.User.CoverImageUrl);
        http.Data.FullName.Should().Be(result.User.FullName);
        http.Data.ImageProfileUrl.Should().NotBe(result.User.ImageProfileUrl);
        http.Data.IsPrivate.Should().Be(result.User.IsPrivate);
        http.Data.Language.Should().NotBe(result.User.Language);
        http.Data.Username.Should().Be(result.User.Username);
        
    }
    
}