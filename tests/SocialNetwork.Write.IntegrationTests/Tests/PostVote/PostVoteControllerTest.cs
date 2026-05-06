using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Modules.PostVote.Dto;
using SocialNetwork.Write.API.Utils.Enums;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.PostVote;

public class PostVoteControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/post-vote";
    
    public PostVoteControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _helper = new HelperTest(Client);
        _output = output;
    }

    [Fact]
    public async Task ShouldCreatePostVote_Success_ChangeUpVoteToDownVote()
    {
        UserTestResult userTest = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userTest);
        await _helper.AddVoteInPost(userTest, postDto.Id, VoteEnum.UPVOTE);

        TogglePostVoteDto dto = new TogglePostVoteDto(postDto.Id, VoteEnum.DOWNVOTE);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.Message.Should().Contain("updated");
        http.TraceId.Should().NotBeNullOrEmpty();

        http.Success.Should().BeTrue();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldDeletePostVote_Success()
    {
        UserTestResult userTest = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(userTest);
        await _helper.AddVoteInPost(userTest, postDto.Id, VoteEnum.UPVOTE);

        TogglePostVoteDto dto = new TogglePostVoteDto(postDto.Id, VoteEnum.UPVOTE);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.Message.Should().Contain("removed");
        http.TraceId.Should().NotBeNullOrEmpty();

        http.Success.Should().BeTrue();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldCreatePostVote_Success()
    {
        UserTestResult userTest = await _helper.CreateNewUser();
        
        PostDto postDto = await _helper.CreatePostAsync(userTest);

        TogglePostVoteDto dto = new TogglePostVoteDto(postDto.Id, VoteEnum.UPVOTE);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.Message.Should().Contain("added");
        http.TraceId.Should().NotBeNullOrEmpty();

        http.Success.Should().BeTrue();

        http.Data.Should().BeNull();

    }
    
    [Fact]
    public async Task ShouldCreatePostVote_Success_DownVote()
    {
        UserTestResult userTest = await _helper.CreateNewUser();
        
        PostDto postDto = await _helper.CreatePostAsync(userTest);

        TogglePostVoteDto dto = new TogglePostVoteDto(postDto.Id, VoteEnum.DOWNVOTE);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();

        http.Success.Should().BeTrue();

        http.Data.Should().BeNull();

    }
    
    [Fact]
    public async Task ShouldCreatePostVote_Fail_NotFound()
    {
        UserTestResult userTest = await _helper.CreateNewUser();
        
        TogglePostVoteDto dto = new TogglePostVoteDto(Guid.NewGuid().ToString(), VoteEnum.UPVOTE);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        _output.WriteLine(message.Content.ReadAsStringAsync().Result);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();

        http.Success.Should().BeFalse();

        http.Data.Should().BeNull();

    }
    
}