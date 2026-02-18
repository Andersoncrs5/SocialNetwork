using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Enums.Reaction;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.dto.Reaction;
using SocialNetwork.Write.IntegrationTests.Config;
using SocialNetwork.Write.IntegrationTests.Tests.Utils.Classes;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Reactions;

public class ReactionControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/reaction";

    public ReactionControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task Should_Create_Reaction()
    {
        UserTestResult result = await _helper.LoginMaster();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result.Tokens.Token);

        CreateReactionDto dto = new CreateReactionDto()
        {
            Name = "reaction" + this._helper.GenerateChars().ToLower(),
            Type = ReactionTypeEnum.Action,
            Active = true,
            DisplayOrder = 1,
            EmojiUnicode = "aaaaaaaa",
            EmojiUrl = "https://github.com/andersoncrs5",
            Visible = true
        };

        HttpResponseMessage message = await Client.PostAsJsonAsync($"{_url}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Created);

        ResponseHttp<ReactionDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<ReactionDto>>();
        
        http.Should().NotBeNull();
        http.Data.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();
        http.DetailsError.Should().BeNull();

        http.Data.Id.Should().NotBeNullOrWhiteSpace();
        http.Data.Name.Should().Be(dto.Name);
        http.Data.Type.Should().Be(dto.Type);
        http.Data.EmojiUrl.Should().Be(dto.EmojiUrl);
        http.Data.EmojiUnicode.Should().Be(dto.EmojiUnicode);
        http.Data.DisplayOrder.Should().Be(dto.DisplayOrder);
        http.Data.Active.Should().Be(dto.Active);
        http.Data.Visible.Should().Be(dto.Visible);
    }

    [Fact]
    public async Task Should_Delete_Reaction()
    {
        UserTestResult userData = await _helper.LoginMaster();
        ReactionDto reactionDto = await _helper.CreateReaction(userData);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", userData.Tokens.Token);
        
        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{reactionDto.Id}");
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNull();
        
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task Should_ReturnNorFound_Delete_Reaction()
    {
        UserTestResult userData = await _helper.LoginMaster();

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", userData.Tokens.Token);
        
        HttpResponseMessage message = await Client.DeleteAsync($"{_url}/{Guid.NewGuid()}");
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNull();
        
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnReactionUpdate()
    {
        UserTestResult userData = await _helper.LoginMaster();
        ReactionDto reactionDto = await _helper.CreateReaction(userData);

        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", userData.Tokens.Token);
        
        UpdateReactionDto dto = new()
        {
            Name = reactionDto.Name + "update",
            Type = ReactionTypeEnum.Emotion,
            Active = !reactionDto.Active,
            DisplayOrder = reactionDto.DisplayOrder + 1,
            EmojiUnicode = reactionDto.EmojiUnicode + "update",
            EmojiUrl = "https://github.com/Andersoncrs5/SocialNetwork.git",
            Visible = !reactionDto.Visible,
        };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{reactionDto.Id}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.OK);

        ResponseHttp<ReactionDto>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<ReactionDto>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Success.Should().BeTrue();
        http.TraceId.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().NotBeNull();
        
        http.Data.Id.Should().Be(reactionDto.Id);
        http.Data.Name.Should().Be(dto.Name);
        http.Data.EmojiUrl.Should().Be(dto.EmojiUrl);
        http.Data.EmojiUnicode.Should().Be(dto.EmojiUnicode);
        http.Data.DisplayOrder.Should().Be(dto.DisplayOrder!.Value);
        http.Data.Type.Should().Be(dto.Type.Value);
        http.Data.Active.Should().Be(dto.Active.Value);
        http.Data.Visible.Should().Be(dto.Visible.Value);
    }

    [Fact]
    public async Task ShouldReturnNotFoundReactionUpdate()
    {
        UserTestResult userData = await _helper.LoginMaster();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", userData.Tokens.Token);
        
        UpdateReactionDto dto = new() { };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{Guid.NewGuid()}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ResponseHttp<object>? http = await message.Content.ReadFromJsonAsync<ResponseHttp<object>>();
        
        http.Should().NotBeNull();
        http.Message.Should().NotBeNullOrWhiteSpace();
        http.Success.Should().BeFalse();
        http.TraceId.Should().NotBeNullOrWhiteSpace();

        http.Data.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnForbReactionUpdate()
    {
        UserTestResult userData = await _helper.CreateNewUser();
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", userData.Tokens.Token);
        
        UpdateReactionDto dto = new() { };
        
        HttpResponseMessage message = await Client.PatchAsJsonAsync($"{_url}/{Guid.NewGuid()}", dto);
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

}