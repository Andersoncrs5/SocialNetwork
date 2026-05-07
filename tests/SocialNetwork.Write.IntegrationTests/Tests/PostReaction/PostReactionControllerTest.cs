using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.DTOs.Reaction;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Modules.PostReactions.Dto;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.PostReaction;

public class PostReactionControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/post-reaction";

    public PostReactionControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task ShouldCreatePostReaction_Success()
    {
        UserTestResult master = await _helper.LoginMaster();
        ReactionDto reactionDto = await _helper.CreateReaction(master);
        
        UserTestResult user = await _helper.CreateNewUser();
        PostDto postDto = await _helper.CreatePostAsync(user);
        
        TogglePostReactionDto dto = new TogglePostReactionDto(PostId:postDto.Id, reactionDto.Id);
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        
        string s = await message.Content.ReadAsStringAsync();
        _output.WriteLine(s);
        
        message.StatusCode.Should().Be(HttpStatusCode.Created);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Message.Should().Contain("added");
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.Success.Should().BeTrue();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldCreatePostReaction_Fail_PostNotFound()
    {
        UserTestResult master = await _helper.LoginMaster();
        ReactionDto reactionDto = await _helper.CreateReaction(master);
        
        UserTestResult user = await _helper.CreateNewUser();
        
        TogglePostReactionDto dto = new TogglePostReactionDto(PostId:reactionDto.Id, reactionDto.Id);
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", user.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}/toggle", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        string s = await message.Content.ReadAsStringAsync();
        _output.WriteLine(s);

        // ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        //
        // http.Should().NotBeNull();
        // http.Message.Should().NotBeNullOrWhiteSpace();
        // http.Message.Should().Contain("added");
        // http.TraceId.Should().NotBeNullOrWhiteSpace();
        // http.Success.Should().BeTrue();
        //
        // http.Data.Should().BeNull();
    }
    
}