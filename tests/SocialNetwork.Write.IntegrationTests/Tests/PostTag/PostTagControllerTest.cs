using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SocialNetwork.Contracts.DTOs.PostTag;
using SocialNetwork.Contracts.DTOs.Tag;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.Posts;
using SocialNetwork.Write.API.dto.PostTag;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.PostTag;

public class PostTagControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/post-tag";
    
    public PostTagControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _helper = new HelperTest(Client);
        _output = output;
    }

    [Fact]
    public async Task Create_PostTag_Success()
    {
        UserTestResult loginMaster = await _helper.LoginMaster();
        UserTestResult userTest = await _helper.CreateNewUser();
        
        TagDto tagDto = await _helper.CreateTag(loginMaster);
        PostDto postDto = await _helper.CreatePostAsync(userTest);

        CreatePostTagDto dto = new ()
        {
            PostId = postDto.Id,
            TagId = tagDto.Id,
        };
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<PostTagDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<PostTagDto>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();
        
        http.Success.Should().BeTrue();

        http.Data.Should().NotBeNull();
        http.Data.Id.Should().NotBeNullOrEmpty();
        http.Data.TagId.Should().Be(dto.TagId);
        http.Data.PostId.Should().Be(dto.PostId);
        
    }
    
    [Fact]
    public async Task ShouldReturnForbiddenBecauseAnotherUserAddTag()
    {
        UserTestResult loginMaster = await _helper.LoginMaster();
        
        UserTestResult userTest = await _helper.CreateNewUser();
        UserTestResult userTest2 = await _helper.CreateNewUser();
        
        TagDto tagDto = await _helper.CreateTag(loginMaster);
        PostDto postDto = await _helper.CreatePostAsync(userTest);

        CreatePostTagDto dto = new ()
        {
            PostId = postDto.Id,
            TagId = tagDto.Id,
        };
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest2.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();
        
        http.Success.Should().BeFalse();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnNotFoundBecauseTagNotExists()
    {
        UserTestResult userTest = await _helper.CreateNewUser();
        
        PostDto postDto = await _helper.CreatePostAsync(userTest);

        CreatePostTagDto dto = new ()
        {
            PostId = postDto.Id,
            TagId = Guid.NewGuid().ToString(),
        };
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();
        
        http.Success.Should().BeFalse();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnNotFoundBecausePostNotExists()
    {
        UserTestResult userTest = await _helper.CreateNewUser();
        
        CreatePostTagDto dto = new ()
        {
            PostId = Guid.NewGuid().ToString(),
            TagId = Guid.NewGuid().ToString(),
        };
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();
        
        http.Success.Should().BeFalse();

        http.Data.Should().BeNull();
    }

    [Fact]
    public async Task ShouldDeleteTagOfPost_Success()
    {
        UserTestResult loginMaster = await _helper.LoginMaster();
        
        UserTestResult userTest = await _helper.CreateNewUser();
        
        TagDto tagDto = await _helper.CreateTag(loginMaster);
        PostDto postDto = await _helper.CreatePostAsync(userTest);

        PostTagDto dto = await _helper.CreateTagToPost(userTest, tagDto, postDto);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);
        
        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{dto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();
        
        http.Success.Should().BeTrue();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnForbBecauseAnotherUser()
    {
        UserTestResult loginMaster = await _helper.LoginMaster();
        
        UserTestResult userTest = await _helper.CreateNewUser();
        UserTestResult userTest2 = await _helper.CreateNewUser();
        
        TagDto tagDto = await _helper.CreateTag(loginMaster);
        PostDto postDto = await _helper.CreatePostAsync(userTest);

        PostTagDto dto = await _helper.CreateTagToPost(userTest, tagDto, postDto);
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest2.Tokens.Token);
        
        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{dto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();
        
        http.Success.Should().BeFalse();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnNotFoundTagOfPost_Success()
    {
        UserTestResult userTest = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userTest.Tokens.Token);
        
        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{Guid.NewGuid()}");
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrEmpty();
        http.TraceId.Should().NotBeNullOrEmpty();
        
        http.Success.Should().BeFalse();

        http.Data.Should().BeNull();
    }
    
}