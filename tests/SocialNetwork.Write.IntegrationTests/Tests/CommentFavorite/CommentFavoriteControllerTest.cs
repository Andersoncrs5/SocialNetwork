using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.Comment;
using SocialNetwork.Contracts.DTOs.CommentFavorite;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.CommentFavorite;

public class CommentFavoriteControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/comment-favorite";

    public CommentFavoriteControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task ShouldCreateCommentFavorite_Success()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsync($"{_url}/{commentDto.Id}/toggle", null);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<CommentFavoriteDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CommentFavoriteDto>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Success.Should().BeTrue();
        
        http.Data.Should().NotBeNull();
        http.Data.CommentId.Should().Be(commentDto.Id);
        http.Data.UserId.Should().Be(user.User.Id);
    }
    
    [Fact]
    public async Task ShouldRemoveCommentFavorite_Success()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage messageCreate = await Client.PostAsync($"{_url}/{commentDto.Id}/toggle", null);
        messageCreate.StatusCode.Should().Be(HttpStatusCode.Created);

        HttpResponseMessage message = await Client.PostAsync($"{_url}/{commentDto.Id}/toggle", null);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CommentFavoriteDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CommentFavoriteDto>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Success.Should().BeTrue();
        
        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnNotFound_WhenCommentNotFound()
    {
        UserTestResult user = await _helper.CreateNewUser();

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsync($"{_url}/{Guid.NewGuid()}/toggle", null);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Success.Should().BeFalse();
        
        http.Data.Should().BeNull();
    }
    
}