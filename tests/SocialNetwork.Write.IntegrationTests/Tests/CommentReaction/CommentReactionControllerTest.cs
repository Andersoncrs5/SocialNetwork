using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.Comment;
using SocialNetwork.Contracts.DTOs.CommentReaction;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.DTOs.Reaction;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Modules.CommentReactions.Dto;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.CommentReaction;


public class CommentReactionControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/comment-reaction";

    public CommentReactionControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task shouldCreateCommentReaction_Success()
    {
        UserTestResult master = await _helper.LoginMaster();
        ReactionDto reactionDto = await _helper.CreateReaction(master);
        
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);
        
        ToggleCommentReactionDto dto = new ToggleCommentReactionDto()
        {
            CommentId = commentDto.Id,
            ReactionId = reactionDto.Id
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);
        
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);

        ResponseHttp<CommentReactionDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CommentReactionDto>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Success.Should().BeTrue();

        http.Data.Should().NotBeNull();
        http.Data.CommentId.Should().Be(commentDto.Id);
        http.Data.ReactionId.Should().Be(reactionDto.Id);
        http.Data.UserId.Should().Be(user.User.Id);
    }
 
    [Fact]
    public async Task shouldCreateCommentReaction_Remove()
    {
        UserTestResult master = await _helper.LoginMaster();
        ReactionDto reactionDto = await _helper.CreateReaction(master);
        
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);
        await _helper.CreateCommentReaction(reactionDto, user, commentDto);
        
        ToggleCommentReactionDto dto = new ToggleCommentReactionDto()
        {
            CommentId = commentDto.Id,
            ReactionId = reactionDto.Id
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);

        ResponseHttp<CommentReactionDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CommentReactionDto>>();
        
        http.Should().NotBeNull();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().Contain("removed");
        http.Success.Should().BeTrue();

        http.Data.Should().BeNull();
    }
 
    [Fact]
    public async Task shouldCreateCommentReaction_Updated()
    {
        UserTestResult master = await _helper.LoginMaster();
        ReactionDto reactionDto = await _helper.CreateReaction(master);
        ReactionDto reactionDto2 = await _helper.CreateReaction(master);
        
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);
        await _helper.CreateCommentReaction(reactionDto, user, commentDto);
        
        ToggleCommentReactionDto dto = new ToggleCommentReactionDto()
        {
            CommentId = commentDto.Id,
            ReactionId = reactionDto2.Id
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<CommentReactionDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<CommentReactionDto>>();
        
        http.Should().NotBeNull();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().Contain("updated");
        http.Success.Should().BeTrue();

        http.Data.Should().NotBeNull();
        http.Data.CommentId.Should().Be(commentDto.Id);
        http.Data.ReactionId.Should().Be(reactionDto2.Id);
        http.Data.UserId.Should().Be(user.User.Id);
    }
 
    [Fact]
    public async Task shouldCreateCommentReaction_Fail_BecauseCommentNotFound()
    {
        UserTestResult master = await _helper.LoginMaster();
        ReactionDto reactionDto = await _helper.CreateReaction(master);
        
        UserTestResult user = await _helper.CreateNewUser();
        
        ToggleCommentReactionDto dto = new ToggleCommentReactionDto()
        {
            CommentId = Guid.NewGuid().ToString(),
            ReactionId = reactionDto.Id
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);
        //
        // ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        //
        // http.Should().NotBeNull();
        // http.Message.Should().NotBeNullOrWhiteSpace();
        // http.TraceId.Should().NotBeNullOrWhiteSpace();
        // http.Success.Should().BeTrue();
        //
        // http.Data.Should().BeNull();
    }
 
    [Fact]
    public async Task shouldCreateCommentReaction_Fail_BecauseReactionNotFound()
    {
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        CommentDto commentDto = await _helper.CreateComment(user, postDto);
        
        ToggleCommentReactionDto dto = new ToggleCommentReactionDto()
        {
            CommentId = commentDto.Id,
            ReactionId = Guid.NewGuid().ToString()
        };
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        //
        // ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        //
        // http.Should().NotBeNull();
        // http.Message.Should().NotBeNullOrWhiteSpace();
        // http.TraceId.Should().NotBeNullOrWhiteSpace();
        // http.Success.Should().BeTrue();
        //
        // http.Data.Should().BeNull();
    }

    
}