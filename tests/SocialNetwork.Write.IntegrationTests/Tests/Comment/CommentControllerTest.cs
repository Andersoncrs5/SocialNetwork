using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.Comment;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.Comment;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Comment;

public class CommentControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/comment";

    public CommentControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task CreateComment_Success()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        CreateCommentDto dto = new()
        {
            PostId = postDto.Id,
            Content = string.Concat(Enumerable.Repeat("AnyContent", 20))
        };

        HttpResponseMessage message = await Client.PostAsJsonAsync(_url, dto);
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<CommentDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CommentDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.Content.Should().Be(dto.Content);
        http.Data.PostId.Should().Be(dto.PostId);
        http.Data.ParentId.Should().BeNull();
        http.Data.UserId.Should().Be(user.User.Id);
    }
    
    [Fact]
    public async Task CreateCommentOnComment_Success()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        CreateCommentDto dto = new()
        {
            PostId = postDto.Id,
            Content = string.Concat(Enumerable.Repeat("AnyContent", 20)),
            ParentId = commentDto.ParentId
        };

        HttpResponseMessage message = await Client.PostAsJsonAsync(_url, dto);
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<CommentDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CommentDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.Content.Should().Be(dto.Content);
        http.Data.PostId.Should().Be(dto.PostId);
        http.Data.ParentId.Should().Be(dto.ParentId);
        http.Data.UserId.Should().Be(user.User.Id);
    }
    
    [Fact]
    public async Task ShouldDeleteComment()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{commentDto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnForbDeleteComment_AnotherUserTriedDelete()
    {
        UserTestResult user = await _helper.CreateNewUser();
        UserTestResult user2 = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user2.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{commentDto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeFalse();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnNotFound_Success()
    {
        UserTestResult user = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{Guid.NewGuid()}");
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeFalse();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldPatchComment()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        UpdateCommentDto dto = new()
        {
            Content = "AnyContentUpdated",
        };

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{commentDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<CommentDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CommentDto>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeTrue();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().NotBeNull();
        http.Data.Id.Should().Be(commentDto.Id);
        http.Data.PostId.Should().Be(commentDto.PostId);
        http.Data.UserId.Should().Be(commentDto.UserId);
        http.Data.ParentId.Should().Be(commentDto.ParentId);
        
        http.Data.Content.Should().Be(dto.Content);
    }
    
    [Fact]
    public async Task ShouldReturnForbPatchComment()
    {
        UserTestResult user = await _helper.CreateNewUser();
        UserTestResult user2 = await _helper.CreateNewUser();
        
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user2.Tokens.Token);

        UpdateCommentDto dto = new()
        {
            Content = "AnyContentUpdated",
        };

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{commentDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeFalse();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnNotFoundPatchComment()
    {
        UserTestResult user = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        UpdateCommentDto dto = new()
        {
            Content = "AnyContentUpdated",
        };

        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{Guid.NewGuid()}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Success.Should().BeFalse();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        
        http.Data.Should().BeNull();
    }

    
}