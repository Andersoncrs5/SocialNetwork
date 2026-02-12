using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.DTOs.PostFavorite;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.PostFavorite;

public class PostFavoriteControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/post-favorite";

    public PostFavoriteControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task ShouldCreatePostFavorite()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsync($"{_url}/{postDto.Id}/toggle", null); 
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<PostFavoriteDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostFavoriteDto>>();
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        
        http.Data.Should().NotBeNull();
        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.PostId.Should().Be(postDto.Id);
        http.Data.UserId.Should().Be(user.User.Id);
    }
    
    [Fact]
    public async Task ShouldReturnNotFound_Success()
    {
        UserTestResult user = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsync($"{_url}/{Guid.NewGuid()}/toggle", null);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        http.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().BeNull();
    }

    [Fact]
    public async Task ShouldRemovePostFavorite_Success()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        PostFavoriteDto postFavoriteDto = await _helper.CreatePostFavorite(user, postDto);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsync($"{_url}/{postDto.Id}/toggle", null); 
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<PostFavoriteDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostFavoriteDto>>();
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        
        http.Data.Should().BeNull();
    }
    
}